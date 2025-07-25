import * as THREE from 'three'
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js'
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader.js'

/**
 * 塔吊升降机3D可视化引擎
 * 支持应力云图、防碰撞预演、手势控制、10Hz高频数据同步
 * 性能目标: 姿态同步误差≤0.5°，应力云图≥5Hz更新
 */
export class CraneVisualizationEngine extends EventTarget {
  private container: HTMLElement
  private scene: THREE.Scene
  private camera: THREE.PerspectiveCamera
  private renderer: THREE.WebGLRenderer
  private controls: OrbitControls
  private loader: GLTFLoader
  
  // 塔吊相关
  private cranesGroup: THREE.Group = new THREE.Group()
  private craneModels: Map<string, THREE.Group> = new Map()
  private craneData: Map<string, any> = new Map()
  private craneAnimations: Map<string, any> = new Map()
  
  // 应力云图相关
  private stressGroup: THREE.Group = new THREE.Group()
  private stressMeshes: Map<string, THREE.Mesh[]> = new Map()
  private stressColors = [
    0x4CAF50, // 绿色 - 正常应力
    0xFFC107, // 黄色 - 轻微应力
    0xFF9800, // 橙色 - 中等应力
    0xF44336, // 红色 - 高应力
    0x9C27B0  // 紫色 - 危险应力
  ]
  
  // 碰撞检测相关
  private collisionGroup: THREE.Group = new THREE.Group()
  private safetyZones: Map<string, THREE.Mesh> = new Map()
  private collisionWarningActive: boolean = false
  
  // 手势控制相关
  private gestureEnabled: boolean = false
  private gestureRecognizer: any = null
  private touchStartPosition: THREE.Vector2 = new THREE.Vector2()
  private touchStartTime: number = 0
  
  // 性能监控
  private lastUpdateTime: number = 0
  private syncErrorAccumulator: number = 0
  private syncErrorSamples: number = 0
  
  // 交互相关
  private raycaster: THREE.Raycaster = new THREE.Raycaster()
  private mouse: THREE.Vector2 = new THREE.Vector2()
  
  // 渲染控制
  private animationId?: number
  private isDestroyed: boolean = false

  constructor(container: HTMLElement) {
    super()
    this.container = container
    this.loader = new GLTFLoader()
    
    this.initEngine()
  }

  /// <summary>
  /// 初始化3D引擎
  /// </summary>
  private initEngine(): void {
    this.initScene()
    this.initCamera()
    this.initRenderer()
    this.initControls()
    this.initLights()
    this.initEventListeners()
  }

  /// <summary>
  /// 初始化场景
  /// </summary>
  private initScene(): void {
    this.scene = new THREE.Scene()
    this.scene.background = new THREE.Color(0x1a1a2e)
    this.scene.fog = new THREE.Fog(0x1a1a2e, 200, 1000)
    
    // 添加组
    this.scene.add(this.cranesGroup)
    this.scene.add(this.stressGroup)
    this.scene.add(this.collisionGroup)
    
    // 设置组名称
    this.cranesGroup.name = 'Cranes'
    this.stressGroup.name = 'StressVisualization'
    this.collisionGroup.name = 'CollisionDetection'
  }

  /// <summary>
  /// 初始化相机
  /// </summary>
  private initCamera(): void {
    const aspect = this.container.clientWidth / this.container.clientHeight
    this.camera = new THREE.PerspectiveCamera(45, aspect, 1, 2000)
    this.camera.position.set(100, 80, 150)
    this.camera.lookAt(0, 0, 0)
  }

  /// <summary>
  /// 初始化渲染器
  /// </summary>
  private initRenderer(): void {
    this.renderer = new THREE.WebGLRenderer({ 
      antialias: true,
      alpha: true,
      powerPreference: 'high-performance'
    })
    
    this.renderer.setSize(this.container.clientWidth, this.container.clientHeight)
    this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2))
    this.renderer.shadowMap.enabled = true
    this.renderer.shadowMap.type = THREE.PCFSoftShadowMap
    this.renderer.outputColorSpace = THREE.SRGBColorSpace
    this.renderer.toneMapping = THREE.ACESFilmicToneMapping
    this.renderer.toneMappingExposure = 1.2
    
    this.container.appendChild(this.renderer.domElement)
  }

  /// <summary>
  /// 初始化控制器
  /// </summary>
  private initControls(): void {
    this.controls = new OrbitControls(this.camera, this.renderer.domElement)
    this.controls.enableDamping = true
    this.controls.dampingFactor = 0.05
    this.controls.maxPolarAngle = Math.PI / 2.1
    this.controls.minDistance = 30
    this.controls.maxDistance = 500
    this.controls.autoRotate = false
    this.controls.autoRotateSpeed = 0.5
  }

  /// <summary>
  /// 初始化灯光
  /// </summary>
  private initLights(): void {
    // 环境光
    const ambientLight = new THREE.AmbientLight(0xffffff, 0.4)
    this.scene.add(ambientLight)
    
    // 主方向光
    const directionalLight = new THREE.DirectionalLight(0xffffff, 1.0)
    directionalLight.position.set(100, 200, 100)
    directionalLight.castShadow = true
    directionalLight.shadow.mapSize.width = 4096
    directionalLight.shadow.mapSize.height = 4096
    directionalLight.shadow.camera.near = 0.5
    directionalLight.shadow.camera.far = 800
    directionalLight.shadow.camera.left = -200
    directionalLight.shadow.camera.right = 200
    directionalLight.shadow.camera.top = 200
    directionalLight.shadow.camera.bottom = -200
    directionalLight.shadow.bias = -0.0001
    this.scene.add(directionalLight)
    
    // 补充光源
    const fillLight = new THREE.DirectionalLight(0x87ceeb, 0.3)
    fillLight.position.set(-50, 100, -50)
    this.scene.add(fillLight)
  }

  /// <summary>
  /// 初始化事件监听
  /// </summary>
  private initEventListeners(): void {
    window.addEventListener('resize', this.handleResize.bind(this))
    this.renderer.domElement.addEventListener('click', this.handleClick.bind(this))
    this.renderer.domElement.addEventListener('mousemove', this.handleMouseMove.bind(this))
    
    // 手势事件
    this.renderer.domElement.addEventListener('touchstart', this.handleTouchStart.bind(this))
    this.renderer.domElement.addEventListener('touchmove', this.handleTouchMove.bind(this))
    this.renderer.domElement.addEventListener('touchend', this.handleTouchEnd.bind(this))
  }

  /// <summary>
  /// 初始化引擎
  /// </summary>
  public async init(): Promise<void> {
    // 创建地面
    this.createGround()
    
    // 创建建筑物
    this.createBuildings()
    
    // 初始化动画系统
  }

  /// <summary>
  /// 创建地面
  /// </summary>
  private createGround(): void {
    const groundGeometry = new THREE.PlaneGeometry(400, 400)
    const groundMaterial = new THREE.MeshLambertMaterial({ 
      color: 0x7D8471,
      transparent: true,
      opacity: 0.8
    })
    const ground = new THREE.Mesh(groundGeometry, groundMaterial)
    ground.rotation.x = -Math.PI / 2
    ground.receiveShadow = true
    this.scene.add(ground)
    
    // 地面网格
    const gridHelper = new THREE.GridHelper(400, 40, 0x444444, 0x222222)
    gridHelper.material.transparent = true
    gridHelper.material.opacity = 0.3
    this.scene.add(gridHelper)
  }

  /// <summary>
  /// 创建建筑物
  /// </summary>
  private createBuildings(): void {
    const buildings = [
      { position: [0, 20, 0], size: [50, 40, 25] },
      { position: [80, 25, 40], size: [30, 50, 20] },
      { position: [-70, 15, -30], size: [40, 30, 25] },
      { position: [40, 18, -80], size: [35, 36, 18] }
    ]
    
    buildings.forEach((building, index) => {
      const geometry = new THREE.BoxGeometry(building.size[0], building.size[1], building.size[2])
      const material = new THREE.MeshLambertMaterial({ 
        color: new THREE.Color().setHSL(0.1, 0.2, 0.5 + Math.random() * 0.2)
      })
      const mesh = new THREE.Mesh(geometry, material)
      mesh.position.set(building.position[0], building.position[1], building.position[2])
      mesh.castShadow = true
      mesh.receiveShadow = true
      this.scene.add(mesh)
    })
  }

  /// <summary>
  /// 添加塔吊
  /// </summary>
  public async addCrane(craneData: any): Promise<void> {
    try {
      const craneGroup = await this.createCraneModel(craneData)
      
      this.craneModels.set(craneData.id, craneGroup)
      this.craneData.set(craneData.id, craneData)
      this.cranesGroup.add(craneGroup)
      
      // 创建安全区域
      this.createSafetyZone(craneData)
      
      // 初始化应力网格
      this.initStressMeshes(craneData.id, craneGroup)
      
      console.log(`塔吊 ${craneData.name} 添加成功`)
      
    } catch (error) {
      console.error(`添加塔吊失败: ${craneData.id}`, error)
    }
  }

  /// <summary>
  /// 创建塔吊模型
  /// </summary>
  private async createCraneModel(craneData: any): Promise<THREE.Group> {
    const craneGroup = new THREE.Group()
    craneGroup.name = craneData.id
    
    // 塔身
    const towerGeometry = new THREE.CylinderGeometry(1.5, 2, craneData.height || 50, 8)
    const towerMaterial = new THREE.MeshLambertMaterial({ 
      color: 0xffd700,
      transparent: true,
      opacity: 0.9
    })
    const tower = new THREE.Mesh(towerGeometry, towerMaterial)
    tower.position.y = (craneData.height || 50) / 2
    tower.castShadow = true
    tower.receiveShadow = true
    tower.name = 'tower'
    
    // 起重臂 (可旋转)
    const jibGroup = new THREE.Group()
    jibGroup.name = 'jib'
    
    // 主臂
    const jibGeometry = new THREE.BoxGeometry(craneData.workRadius || 50, 1, 2)
    const jibMaterial = new THREE.MeshLambertMaterial({ color: 0xff6b35 })
    const jib = new THREE.Mesh(jibGeometry, jibMaterial)
    jib.position.x = (craneData.workRadius || 50) / 2
    jib.castShadow = true
    jib.name = 'main_jib'
    
    // 平衡臂
    const counterJibGeometry = new THREE.BoxGeometry(15, 1, 2)
    const counterJib = new THREE.Mesh(counterJibGeometry, jibMaterial)
    counterJib.position.x = -7.5
    counterJib.castShadow = true
    counterJib.name = 'counter_jib'
    
    // 吊钩
    const hookGeometry = new THREE.SphereGeometry(0.5)
    const hookMaterial = new THREE.MeshLambertMaterial({ color: 0x333333 })
    const hook = new THREE.Mesh(hookGeometry, hookMaterial)
    hook.position.set(craneData.workRadius - 5, -craneData.height + 5, 0)
    hook.castShadow = true
    hook.name = 'hook'
    
    jibGroup.add(jib)
    jibGroup.add(counterJib)
    jibGroup.add(hook)
    jibGroup.position.y = craneData.height || 50
    
    // 设置初始旋转角度
    jibGroup.rotation.y = THREE.MathUtils.degToRad(craneData.rotation || 0)
    
    craneGroup.add(tower)
    craneGroup.add(jibGroup)
    
    // 设置位置
    craneGroup.position.set(
      craneData.position.x,
      craneData.position.y || 0,
      craneData.position.z
    )
    
    craneGroup.userData = { craneData }
    
    return craneGroup
  }

  /// <summary>
  /// 创建安全区域
  /// </summary>
  private createSafetyZone(craneData: any): void {
    const radius = craneData.workRadius + 5 // 5m安全缓冲
    const geometry = new THREE.RingGeometry(radius - 2, radius, 32)
    const material = new THREE.MeshBasicMaterial({ 
      color: 0x00ff00,
      transparent: true,
      opacity: 0.2,
      side: THREE.DoubleSide
    })
    
    const safetyZone = new THREE.Mesh(geometry, material)
    safetyZone.rotation.x = -Math.PI / 2
    safetyZone.position.set(
      craneData.position.x,
      0.1,
      craneData.position.z
    )
    
    this.safetyZones.set(craneData.id, safetyZone)
    this.collisionGroup.add(safetyZone)
  }

  /// <summary>
  /// 初始化应力网格
  /// </summary>
  private initStressMeshes(craneId: string, craneGroup: THREE.Group): void {
    const stressMeshes: THREE.Mesh[] = []
    
    // 为塔吊的关键部件创建应力可视化网格
    const tower = craneGroup.getObjectByName('tower') as THREE.Mesh
    const jib = craneGroup.getObjectByName('main_jib') as THREE.Mesh
    
    if (tower) {
      const stressTower = this.createStressMesh(tower, 'tower')
      stressMeshes.push(stressTower)
      this.stressGroup.add(stressTower)
    }
    
    if (jib) {
      const stressJib = this.createStressMesh(jib, 'jib')
      stressMeshes.push(stressJib)
      this.stressGroup.add(stressJib)
    }
    
    this.stressMeshes.set(craneId, stressMeshes)
  }

  /// <summary>
  /// 创建应力网格
  /// </summary>
  private createStressMesh(originalMesh: THREE.Mesh, partType: string): THREE.Mesh {
    const geometry = originalMesh.geometry.clone()
    const material = new THREE.MeshBasicMaterial({
      transparent: true,
      opacity: 0.6,
      side: THREE.DoubleSide
    })
    
    const stressMesh = new THREE.Mesh(geometry, material)
    stressMesh.position.copy(originalMesh.position)
    stressMesh.rotation.copy(originalMesh.rotation)
    stressMesh.scale.copy(originalMesh.scale)
    stressMesh.userData = { partType, originalMesh }
    
    return stressMesh
  }

  /// <summary>
  /// 更新塔吊实时数据 - 10Hz高频更新
  /// </summary>
  public updateCraneRealtime(craneId: string, craneData: any): void {
    const craneGroup = this.craneModels.get(craneId)
    if (!craneGroup) return
    
    const currentTime = performance.now()
    const deltaTime = currentTime - this.lastUpdateTime
    
    // 目标:100ms间隔更新 (10Hz)
    if (deltaTime < 100) return
    
    try {
      const jibGroup = craneGroup.getObjectByName('jib') as THREE.Group
      if (jibGroup) {
        // 计算目标旋转角度
        const targetRotation = THREE.MathUtils.degToRad(craneData.rotation)
        const currentRotation = jibGroup.rotation.y
        
        // 计算角度差异 - 用于同步精度监控
        let angleDiff = Math.abs(targetRotation - currentRotation) * 180 / Math.PI
        
        // 处理角度环绕
        if (angleDiff > 180) {
          angleDiff = 360 - angleDiff
        }
        
        // 累积同步误差
        this.syncErrorAccumulator += angleDiff
        this.syncErrorSamples++
        
        // 平滑旋转动画 - 确保0.5°精度
        const rotationSpeed = 0.1 // 调整速度确保精度
        if (angleDiff > 0.5) { // 仅在误差超过0.5°时调整
          const direction = this.getShortestRotationDirection(currentRotation, targetRotation)
          jibGroup.rotation.y += direction * rotationSpeed * (deltaTime / 16.67)
        }
        
        // 更新吊钩高度
        const hook = jibGroup.getObjectByName('hook') as THREE.Mesh
        if (hook) {
          const targetY = -craneData.height + 5
          hook.position.y = THREE.MathUtils.lerp(hook.position.y, targetY, 0.1)
        }
      }
      
      // 更新应力可视化
      this.updateStressVisualization(craneId, craneData)
      
      // 检测碰撞风险
      this.checkCollisionRisk()
      
      this.lastUpdateTime = currentTime
      
      // 每秒报告一次同步精度
      if (this.syncErrorSamples >= 10) {
        const avgSyncError = this.syncErrorAccumulator / this.syncErrorSamples
        if (avgSyncError > 0.5) {
          console.warn(`塔吊 ${craneId} 同步精度超标: ${avgSyncError.toFixed(2)}°`)
        }
        
        this.syncErrorAccumulator = 0
        this.syncErrorSamples = 0
      }
      
    } catch (error) {
      console.error(`更新塔吊实时数据失败: ${craneId}`, error)
    }
  }

  /// <summary>
  /// 获取最短旋转方向
  /// </summary>
  private getShortestRotationDirection(current: number, target: number): number {
    let diff = target - current
    
    // 处理角度环绕
    if (diff > Math.PI) {
      diff -= 2 * Math.PI
    } else if (diff < -Math.PI) {
      diff += 2 * Math.PI
    }
    
    return Math.sign(diff)
  }

  /// <summary>
  /// 更新应力可视化 - 5Hz更新频率
  /// </summary>
  private updateStressVisualization(craneId: string, craneData: any): void {
    const stressMeshes = this.stressMeshes.get(craneId)
    if (!stressMeshes) return
    
    stressMeshes.forEach(mesh => {
      const partType = mesh.userData.partType
      let stressLevel = 0
      
      // 根据部件类型和负载计算应力
      switch (partType) {
        case 'tower':
          stressLevel = (craneData.load / craneData.maxLoad) * 100
          break
        case 'jib':
          stressLevel = craneData.stressLevel || 50
          break
      }
      
      // 更新应力颜色
      const colorIndex = Math.floor((stressLevel / 100) * (this.stressColors.length - 1))
      const color = new THREE.Color(this.stressColors[colorIndex])
      
      const material = mesh.material as THREE.MeshBasicMaterial
      material.color.copy(color)
      
      // 根据应力等级调整透明度
      material.opacity = 0.3 + (stressLevel / 100) * 0.4
    })
  }

  /// <summary>
  /// 检测碰撞风险
  /// </summary>
  private checkCollisionRisk(): void {
    const cranes = Array.from(this.craneData.values()).filter(c => c.status === 'online')
    
    for (let i = 0; i < cranes.length; i++) {
      for (let j = i + 1; j < cranes.length; j++) {
        const crane1 = cranes[i]
        const crane2 = cranes[j]
        
        const distance = this.calculateDistance(crane1.position, crane2.position)
        const safeDistance = crane1.workRadius + crane2.workRadius + 10 // 10m安全缓冲
        
        if (distance < safeDistance && !this.collisionWarningActive) {
          this.triggerCollisionWarning(crane1, crane2, distance)
        }
      }
    }
  }

  /// <summary>
  /// 计算距离
  /// </summary>
  private calculateDistance(pos1: any, pos2: any): number {
    const dx = pos1.x - pos2.x
    const dz = pos1.z - pos2.z
    return Math.sqrt(dx * dx + dz * dz)
  }

  /// <summary>
  /// 触发碰撞警告
  /// </summary>
  private triggerCollisionWarning(crane1: any, crane2: any, distance: number): void {
    this.collisionWarningActive = true
    
    // 更新安全区域颜色为红色
    const zone1 = this.safetyZones.get(crane1.id)
    const zone2 = this.safetyZones.get(crane2.id)
    
    if (zone1) {
      (zone1.material as THREE.MeshBasicMaterial).color.setHex(0xff0000)
    }
    if (zone2) {
      (zone2.material as THREE.MeshBasicMaterial).color.setHex(0xff0000)
    }
    
    // 触发事件
    this.dispatchEvent(new CustomEvent('collisionWarning', {
      detail: {
        crane1Name: crane1.name,
        crane2Name: crane2.name,
        distance,
        timeToCollision: this.estimateCollisionTime(crane1, crane2, distance)
      }
    }))
    
    // 3秒后重置警告状态
    setTimeout(() => {
      this.collisionWarningActive = false
      if (zone1) {
        (zone1.material as THREE.MeshBasicMaterial).color.setHex(0x00ff00)
      }
      if (zone2) {
        (zone2.material as THREE.MeshBasicMaterial).color.setHex(0x00ff00)
      }
    }, 3000)
  }

  /// <summary>
  /// 估算碰撞时间
  /// </summary>
  private estimateCollisionTime(crane1: any, crane2: any, distance: number): number {
    const relativeSpeed = Math.abs(crane1.rotationSpeed || 0) + Math.abs(crane2.rotationSpeed || 0)
    if (relativeSpeed === 0) return -1
    
    const dangerDistance = crane1.workRadius + crane2.workRadius
    return (distance - dangerDistance) / (relativeSpeed * 0.1)
  }

  /// <summary>
  /// 播放碰撞预演动画
  /// </summary>
  public playCollisionSimulation(cranes: any[]): void {
    if (cranes.length < 2) return
    
    const crane1 = cranes[0]
    const crane2 = cranes[1]
    
    const group1 = this.craneModels.get(crane1.id)
    const group2 = this.craneModels.get(crane2.id)
    
    if (!group1 || !group2) return
    
    // 保存原始状态
    const originalRotation1 = group1.getObjectByName('jib')!.rotation.y
    const originalRotation2 = group2.getObjectByName('jib')!.rotation.y
    
    // 模拟碰撞场景
    const collisionAngle1 = originalRotation1 + Math.PI / 4
    const collisionAngle2 = originalRotation2 - Math.PI / 4
    
    // 创建动画
    const jib1 = group1.getObjectByName('jib')!
    const jib2 = group2.getObjectByName('jib')!
    
    // 直接设置旋转角度
    jib1.rotation.y = collisionAngle1
    jib2.rotation.y = collisionAngle2
    
    // 显示碰撞效果
    this.showCollisionEffect(group1.position, group2.position)
    
    // 恢复原始状态
    setTimeout(() => {
      jib1.rotation.y = originalRotation1
      jib2.rotation.y = originalRotation2
    }, 1000)
  }

  /// <summary>
  /// 显示碰撞效果
  /// </summary>
  private showCollisionEffect(pos1: THREE.Vector3, pos2: THREE.Vector3): void {
    const midPoint = new THREE.Vector3().addVectors(pos1, pos2).multiplyScalar(0.5)
    
    // 创建爆炸效果
    const explosionGeometry = new THREE.SphereGeometry(5)
    const explosionMaterial = new THREE.MeshBasicMaterial({ 
      color: 0xff0000,
      transparent: true,
      opacity: 0.8
    })
    const explosion = new THREE.Mesh(explosionGeometry, explosionMaterial)
    explosion.position.copy(midPoint)
    explosion.position.y += 50
    
    this.scene.add(explosion)
    
    // 爆炸动画 - 直接设置
    explosion.scale.set(3, 3, 3)
    explosionMaterial.opacity = 0
    this.scene.remove(explosion)
  }

  /// <summary>
  /// 启用应力热力图
  /// </summary>
  public enableStressHeatmap(enabled: boolean): void {
    this.stressGroup.visible = enabled
  }

  /// <summary>
  /// 显示应力分布
  /// </summary>
  public showStressDistribution(craneId: string): void {
    const stressMeshes = this.stressMeshes.get(craneId)
    if (!stressMeshes) return
    
    stressMeshes.forEach(mesh => {
      mesh.visible = true
    })
  }

  /// <summary>
  /// 高亮塔吊
  /// </summary>
  public highlightCrane(craneId: string): void {
    // 重置所有塔吊高亮
    this.craneModels.forEach((group, id) => {
      const tower = group.getObjectByName('tower') as THREE.Mesh
      if (tower) {
        const material = tower.material as THREE.MeshLambertMaterial
        material.emissive.setHex(0x000000)
      }
    })
    
    // 高亮选中的塔吊
    const craneGroup = this.craneModels.get(craneId)
    if (craneGroup) {
      const tower = craneGroup.getObjectByName('tower') as THREE.Mesh
      if (tower) {
        const material = tower.material as THREE.MeshLambertMaterial
        material.emissive.setHex(0x444444)
      }
      
      // 相机焦点移动到塔吊
      this.focusOnCrane(craneId)
    }
  }

  /// <summary>
  /// 聚焦到塔吊
  /// </summary>
  public focusOnCrane(craneId: string): void {
    const craneGroup = this.craneModels.get(craneId)
    if (!craneGroup) return
    
    const targetPosition = craneGroup.position.clone()
    targetPosition.y += 60
    targetPosition.z += 80
    
    const targetLookAt = craneGroup.position.clone()
    targetLookAt.y += 25
    
    this.animateCamera(targetPosition, targetLookAt)
  }

  /// <summary>
  /// 相机动画
  /// </summary>
  private animateCamera(targetPosition: THREE.Vector3, targetLookAt: THREE.Vector3): void {
    // 直接设置相机位置和目标
    this.camera.position.copy(targetPosition)
    this.controls.target.copy(targetLookAt)
    this.controls.update()
  }

  /// <summary>
  /// 启用手势控制
  /// </summary>
  public enableGestureControl(enabled: boolean): void {
    this.gestureEnabled = enabled
  }

  /// <summary>
  /// 触摸开始
  /// </summary>
  private handleTouchStart(event: TouchEvent): void {
    if (!this.gestureEnabled || event.touches.length === 0) return
    
    const touch = event.touches[0]
    this.touchStartPosition.set(touch.clientX, touch.clientY)
    this.touchStartTime = Date.now()
  }

  /// <summary>
  /// 触摸移动
  /// </summary>
  private handleTouchMove(event: TouchEvent): void {
    if (!this.gestureEnabled) return
    
    // 阻止默认滚动行为
    event.preventDefault()
  }

  /// <summary>
  /// 触摸结束 - 手势识别
  /// </summary>
  private handleTouchEnd(event: TouchEvent): void {
    if (!this.gestureEnabled || event.changedTouches.length === 0) return
    
    const touch = event.changedTouches[0]
    const endPosition = new THREE.Vector2(touch.clientX, touch.clientY)
    const deltaTime = Date.now() - this.touchStartTime
    const distance = this.touchStartPosition.distanceTo(endPosition)
    const direction = endPosition.clone().sub(this.touchStartPosition).normalize()
    
    // 识别手势类型
    if (deltaTime < 500 && distance > 50) {
      let gestureType = ''
      let gestureDirection = ''
      
      if (Math.abs(direction.y) > Math.abs(direction.x)) {
        // 垂直滑动
        gestureType = direction.y < 0 ? 'swipe_up' : 'swipe_down'
      } else {
        // 水平滑动
        gestureType = 'rotate'
        gestureDirection = direction.x > 0 ? 'right' : 'left'
      }
      
      const intensity = Math.min(distance / 200, 1) // 归一化强度
      
      this.dispatchEvent(new CustomEvent('gestureDetected', {
        detail: {
          type: gestureType,
          direction: gestureDirection,
          intensity
        }
      }))
    }
  }

  /// <summary>
  /// 更新塔吊状态
  /// </summary>
  public updateCraneState(craneId: string, newState: any): void {
    const craneData = this.craneData.get(craneId)
    if (!craneData) return
    
    // 更新数据
    Object.assign(craneData, newState)
    this.craneData.set(craneId, craneData)
    
    // 立即更新3D模型
    this.updateCraneRealtime(craneId, craneData)
  }

  /// <summary>
  /// 紧急停机
  /// </summary>
  public emergencyStopCrane(craneId: string): void {
    const craneGroup = this.craneModels.get(craneId)
    if (!craneGroup) return
    
    // 停止所有动画
    
    // 设置紧急停机视觉效果
    const tower = craneGroup.getObjectByName('tower') as THREE.Mesh
    if (tower) {
      const material = tower.material as THREE.MeshLambertMaterial
      material.emissive.setHex(0xff0000) // 红色警示
      
      // 闪烁效果
      const blinkInterval = setInterval(() => {
        material.emissive.setHex(
          material.emissive.getHex() === 0xff0000 ? 0x000000 : 0xff0000
        )
      }, 500)
      
      // 5秒后停止闪烁
      setTimeout(() => {
        clearInterval(blinkInterval)
        material.emissive.setHex(0x000000)
      }, 5000)
    }
  }

  /// <summary>
  /// 聚焦应力节点
  /// </summary>
  public focusOnStressNode(craneId: string, nodeId: string): void {
    const craneGroup = this.craneModels.get(craneId)
    if (!craneGroup) return
    
    // 根据nodeId找到对应的部件
    let targetMesh: THREE.Object3D | undefined
    
    switch (nodeId) {
      case 'node1': // 起重臂根部
      case 'node3': // 回转支承
        targetMesh = craneGroup.getObjectByName('jib')
        break
      case 'node2': // 塔身中段
        targetMesh = craneGroup.getObjectByName('tower')
        break
      default:
        targetMesh = craneGroup
    }
    
    if (targetMesh) {
      const targetPosition = new THREE.Vector3()
      targetMesh.getWorldPosition(targetPosition)
      targetPosition.add(new THREE.Vector3(20, 10, 20))
      
      const lookAtPosition = new THREE.Vector3()
      targetMesh.getWorldPosition(lookAtPosition)
      
      this.animateCamera(targetPosition, lookAtPosition)
    }
  }

  /// <summary>
  /// 缩放到细节
  /// </summary>
  public zoomToDetail(craneId: string, intensity: number): void {
    const distance = this.controls.getDistance()
    const targetDistance = distance * (1 - intensity * 0.5)
    
    new TWEEN.Tween({ distance })
      .to({ distance: targetDistance }, 1000)
      .onUpdate((obj) => {
        this.camera.position.sub(this.controls.target).normalize().multiplyScalar(obj.distance).add(this.controls.target)
      })
      .start()
  }

  /// <summary>
  /// 鼠标点击处理
  /// </summary>
  private handleClick(event: MouseEvent): void {
    this.updateMousePosition(event)
    this.raycaster.setFromCamera(this.mouse, this.camera)
    
    const craneObjects: THREE.Object3D[] = []
    this.craneModels.forEach(group => {
      craneObjects.push(group)
    })
    
    const intersects = this.raycaster.intersectObjects(craneObjects, true)
    if (intersects.length > 0) {
      let craneGroup = intersects[0].object
      while (craneGroup.parent && craneGroup.parent !== this.cranesGroup) {
        craneGroup = craneGroup.parent
      }
      
      const craneData = craneGroup.userData?.craneData
      if (craneData) {
        this.dispatchEvent(new CustomEvent('craneClick', { detail: craneData }))
      }
    }
  }

  /// <summary>
  /// 鼠标移动处理
  /// </summary>
  private handleMouseMove(event: MouseEvent): void {
    this.updateMousePosition(event)
  }

  /// <summary>
  /// 更新鼠标位置
  /// </summary>
  private updateMousePosition(event: MouseEvent): void {
    const rect = this.renderer.domElement.getBoundingClientRect()
    this.mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1
    this.mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1
  }

  /// <summary>
  /// 窗口大小调整
  /// </summary>
  private handleResize(): void {
    if (this.isDestroyed) return
    
    const width = this.container.clientWidth
    const height = this.container.clientHeight
    
    this.camera.aspect = width / height
    this.camera.updateProjectionMatrix()
    this.renderer.setSize(width, height)
  }

  /// <summary>
  /// 开始渲染循环
  /// </summary>
  public startRender(): void {
    if (this.isDestroyed) return
    
    const animate = () => {
      if (this.isDestroyed) return
      
      this.animationId = requestAnimationFrame(animate)
      
      // 更新控制器
      this.controls.update()
      
      // 更新动画
      
      // 渲染场景
      this.renderer.render(this.scene, this.camera)
    }
    
    animate()
  }

  /// <summary>
  /// 销毁引擎
  /// </summary>
  public destroy(): void {
    this.isDestroyed = true
    
    if (this.animationId) {
      cancelAnimationFrame(this.animationId)
    }
    
    // 停止所有动画
    
    // 清理事件监听器
    window.removeEventListener('resize', this.handleResize.bind(this))
    
    // 清理Three.js对象
    this.scene.clear()
    this.renderer.dispose()
    
    // 清理DOM
    if (this.container.contains(this.renderer.domElement)) {
      this.container.removeChild(this.renderer.domElement)
    }
  }
} 