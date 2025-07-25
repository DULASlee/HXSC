import * as THREE from 'three'
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js'
import { InstancedMesh } from 'three'

/**
 * 实名制考勤3D可视化引擎
 * 支持动态热力图、人员轨迹回放、三维人证核验
 * 性能目标: 千人员工数据加载≤1.5s，轨迹回放≥30fps
 */
export class AttendanceVisualizationEngine extends EventTarget {
  private container: HTMLElement
  private scene: THREE.Scene
  private camera: THREE.PerspectiveCamera
  private renderer: THREE.WebGLRenderer
  private controls: OrbitControls
  
  // 人员相关
  private personnelGroup: THREE.Group = new THREE.Group()
  private personnelInstances: Map<string, THREE.InstancedMesh> = new Map()
  private personnelData: Map<string, any> = new Map()
  
  // 热力图相关
  private heatmapGroup: THREE.Group = new THREE.Group()
  private heatmapCanvas: HTMLCanvasElement
  private heatmapContext: CanvasRenderingContext2D
  private heatmapTexture: THREE.CanvasTexture
  
  // 轨迹相关
  private trajectoryGroup: THREE.Group = new THREE.Group()
  private trajectoryLine: THREE.Line | null = null
  private trajectoryAnimation: any = null
  private trajectoryMarker: THREE.Mesh | null = null
  
  // 区域相关
  private areasGroup: THREE.Group = new THREE.Group()
  private workAreas: Map<string, THREE.Mesh> = new Map()
  
  // 交互相关
  private raycaster: THREE.Raycaster = new THREE.Raycaster()
  private mouse: THREE.Vector2 = new THREE.Vector2()
  
  // 性能监控
  private frameCount: number = 0
  private lastTime: number = 0
  private performanceStats = {
    fps: 60,
    drawCalls: 0,
    triangles: 0,
    memory: 0
  }
  
  // 渲染控制
  private animationId?: number
  private isDestroyed: boolean = false

  constructor(container: HTMLElement) {
    super()
    this.container = container
    
    // 创建热力图画布
    this.heatmapCanvas = document.createElement('canvas')
    this.heatmapCanvas.width = 512
    this.heatmapCanvas.height = 512
    this.heatmapContext = this.heatmapCanvas.getContext('2d')!
    this.heatmapTexture = new THREE.CanvasTexture(this.heatmapCanvas)
    
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
    this.createWorkAreas()
  }

  /// <summary>
  /// 初始化场景
  /// </summary>
  private initScene(): void {
    this.scene = new THREE.Scene()
    this.scene.background = new THREE.Color(0x1a1a2e)
    this.scene.fog = new THREE.Fog(0x1a1a2e, 100, 1000)
    
    // 添加组
    this.scene.add(this.personnelGroup)
    this.scene.add(this.heatmapGroup)
    this.scene.add(this.trajectoryGroup)
    this.scene.add(this.areasGroup)
    
    // 设置组名称
    this.personnelGroup.name = 'Personnel'
    this.heatmapGroup.name = 'Heatmap'
    this.trajectoryGroup.name = 'Trajectory'
    this.areasGroup.name = 'WorkAreas'
  }

  /// <summary>
  /// 初始化相机
  /// </summary>
  private initCamera(): void {
    const aspect = this.container.clientWidth / this.container.clientHeight
    this.camera = new THREE.PerspectiveCamera(60, aspect, 1, 2000)
    this.camera.position.set(0, 80, 120)
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
    
    // 性能优化设置
    this.renderer.info.autoReset = false
    
    this.container.appendChild(this.renderer.domElement)
  }

  /// <summary>
  /// 初始化控制器
  /// </summary>
  private initControls(): void {
    this.controls = new OrbitControls(this.camera, this.renderer.domElement)
    this.controls.enableDamping = true
    this.controls.dampingFactor = 0.05
    this.controls.maxPolarAngle = Math.PI / 2.2
    this.controls.minDistance = 20
    this.controls.maxDistance = 300
    this.controls.autoRotate = false
  }

  /// <summary>
  /// 初始化灯光
  /// </summary>
  private initLights(): void {
    // 环境光
    const ambientLight = new THREE.AmbientLight(0xffffff, 0.6)
    this.scene.add(ambientLight)
    
    // 主方向光
    const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8)
    directionalLight.position.set(50, 100, 50)
    directionalLight.castShadow = true
    directionalLight.shadow.mapSize.width = 2048
    directionalLight.shadow.mapSize.height = 2048
    directionalLight.shadow.camera.near = 0.5
    directionalLight.shadow.camera.far = 500
    directionalLight.shadow.camera.left = -100
    directionalLight.shadow.camera.right = 100
    directionalLight.shadow.camera.top = 100
    directionalLight.shadow.camera.bottom = -100
    this.scene.add(directionalLight)
  }

  /// <summary>
  /// 初始化事件监听
  /// </summary>
  private initEventListeners(): void {
    window.addEventListener('resize', this.handleResize.bind(this))
    this.renderer.domElement.addEventListener('click', this.handleClick.bind(this))
    this.renderer.domElement.addEventListener('mousemove', this.handleMouseMove.bind(this))
  }

  /// <summary>
  /// 创建工作区域
  /// </summary>
  private createWorkAreas(): void {
    const areas = [
      { id: 'area1', name: '主体结构区', position: [0, 0, 0], size: [60, 40] },
      { id: 'area2', name: '材料堆放区', position: [80, 0, 20], size: [30, 30] },
      { id: 'area3', name: '办公生活区', position: [-60, 0, -30], size: [40, 25] },
      { id: 'area4', name: '设备存放区', position: [20, 0, -70], size: [35, 20] }
    ]
    
    areas.forEach(area => {
      const geometry = new THREE.PlaneGeometry(area.size[0], area.size[1])
      const material = new THREE.MeshLambertMaterial({ 
        color: 0x4CAF50,
        transparent: true,
        opacity: 0.3,
        side: THREE.DoubleSide
      })
      
      const mesh = new THREE.Mesh(geometry, material)
      mesh.position.set(area.position[0], 0.1, area.position[2])
      mesh.rotation.x = -Math.PI / 2
      mesh.userData = { area }
      
      this.workAreas.set(area.id, mesh)
      this.areasGroup.add(mesh)
    })
  }

  /// <summary>
  /// 初始化引擎
  /// </summary>
  public async init(): Promise<void> {
    // 创建地面
    this.createGround()
    
    // 创建建筑物
    this.createBuildings()
    
    // 初始化热力图
    this.initHeatmap()
  }

  /// <summary>
  /// 创建地面
  /// </summary>
  private createGround(): void {
    const groundGeometry = new THREE.PlaneGeometry(300, 300)
    const groundMaterial = new THREE.MeshLambertMaterial({ color: 0x7D8471 })
    const ground = new THREE.Mesh(groundGeometry, groundMaterial)
    ground.rotation.x = -Math.PI / 2
    ground.receiveShadow = true
    this.scene.add(ground)
  }

  /// <summary>
  /// 创建建筑物
  /// </summary>
  private createBuildings(): void {
    const buildings = [
      { position: [0, 15, 0], size: [40, 30, 20] },
      { position: [60, 20, 30], size: [25, 40, 15] },
      { position: [-50, 12, -20], size: [30, 24, 18] }
    ]
    
    buildings.forEach((building, index) => {
      const geometry = new THREE.BoxGeometry(building.size[0], building.size[1], building.size[2])
      const material = new THREE.MeshLambertMaterial({ color: 0x8B7D6B })
      const mesh = new THREE.Mesh(geometry, material)
      mesh.position.set(building.position[0], building.position[1], building.position[2])
      mesh.castShadow = true
      mesh.receiveShadow = true
      this.scene.add(mesh)
    })
  }

  /// <summary>
  /// 初始化热力图
  /// </summary>
  private initHeatmap(): void {
    // 创建热力图平面
    const heatmapGeometry = new THREE.PlaneGeometry(200, 200)
    const heatmapMaterial = new THREE.MeshBasicMaterial({
      map: this.heatmapTexture,
      transparent: true,
      opacity: 0.6
    })
    
    const heatmapMesh = new THREE.Mesh(heatmapGeometry, heatmapMaterial)
    heatmapMesh.rotation.x = -Math.PI / 2
    heatmapMesh.position.y = 0.5
    
    this.heatmapGroup.add(heatmapMesh)
  }

  /// <summary>
  /// 更新人员数据 - 性能优化使用InstancedMesh
  /// </summary>
  public updatePersonnelData(personnelList: any[]): void {
    const startTime = performance.now()
    
    // 清除现有人员
    this.personnelGroup.clear()
    this.personnelInstances.clear()
    this.personnelData.clear()
    
    if (!personnelList || personnelList.length === 0) return
    
    // 按工种分组人员
    const personnelByProfession = new Map<string, any[]>()
    personnelList.forEach(person => {
      const profession = person.profession || 'unknown'
      if (!personnelByProfession.has(profession)) {
        personnelByProfession.set(profession, [])
      }
      personnelByProfession.get(profession)!.push(person)
    })
    
    // 为每个工种创建InstancedMesh
    personnelByProfession.forEach((persons, profession) => {
      this.createPersonnelInstanced(persons, profession)
    })
    
    const endTime = performance.now()
    console.log(`人员数据更新完成，${personnelList.length}人，耗时: ${(endTime - startTime).toFixed(2)}ms`)
    
    // 更新热力图
    this.updateHeatmapData(personnelList)
  }

  /// <summary>
  /// 创建实例化人员网格 - 高性能渲染
  /// </summary>
  private createPersonnelInstanced(persons: any[], profession: string): void {
    const count = persons.length
    if (count === 0) return
    
    // 人员几何体 - 简化为胶囊体
    const geometry = new THREE.CapsuleGeometry(0.5, 1.5, 4, 8)
    
    // 根据工种设置颜色
    const professionColors = {
      '钢筋工': 0xFF6B35,
      '混凝土工': 0x4ECDC4,
      '架子工': 0xFFE66D,
      '电工': 0x45B7D1,
      '安全员': 0xF38BA8,
      'unknown': 0x96CEB4
    }
    
    const color = professionColors[profession as keyof typeof professionColors] || professionColors.unknown
    const material = new THREE.MeshLambertMaterial({ color })
    
    // 创建实例化网格
    const instancedMesh = new THREE.InstancedMesh(geometry, material, count)
    instancedMesh.instanceMatrix.setUsage(THREE.DynamicDrawUsage)
    instancedMesh.castShadow = true
    instancedMesh.userData.profession = profession
    
    // 设置每个实例的位置
    const matrix = new THREE.Matrix4()
    persons.forEach((person, index) => {
      const position = person.position
      matrix.setPosition(position.x, 1.5, position.z)
      instancedMesh.setMatrixAt(index, matrix)
      
      // 存储人员数据
      this.personnelData.set(`${profession}_${index}`, person)
    })
    
    instancedMesh.instanceMatrix.needsUpdate = true
    
    this.personnelInstances.set(profession, instancedMesh)
    this.personnelGroup.add(instancedMesh)
  }

  /// <summary>
  /// 更新热力图数据
  /// </summary>
  public updateHeatmap(personnelList: any[], mode: 'density' | 'worktype' = 'density'): void {
    if (!this.heatmapContext) return
    
    // 清除画布
    this.heatmapContext.clearRect(0, 0, this.heatmapCanvas.width, this.heatmapCanvas.height)
    
    if (mode === 'density') {
      this.renderDensityHeatmap(personnelList)
    } else {
      this.renderWorktypeHeatmap(personnelList)
    }
    
    // 更新纹理
    this.heatmapTexture.needsUpdate = true
  }

  /// <summary>
  /// 渲染密度热力图
  /// </summary>
  private renderDensityHeatmap(personnelList: any[]): void {
    const ctx = this.heatmapContext
    const canvas = this.heatmapCanvas
    
    // 创建密度网格
    const gridSize = 32
    const cellSize = canvas.width / gridSize
    const densityGrid = Array(gridSize).fill(null).map(() => Array(gridSize).fill(0))
    
    // 统计每个网格的人员数量
    personnelList.forEach(person => {
      const x = Math.floor(((person.position.x + 100) / 200) * gridSize)
      const z = Math.floor(((person.position.z + 100) / 200) * gridSize)
      
      if (x >= 0 && x < gridSize && z >= 0 && z < gridSize) {
        densityGrid[z][x]++
      }
    })
    
    // 查找最大密度用于归一化
    const maxDensity = Math.max(...densityGrid.flat())
    if (maxDensity === 0) return
    
    // 渲染热力图
    for (let z = 0; z < gridSize; z++) {
      for (let x = 0; x < gridSize; x++) {
        const density = densityGrid[z][x] / maxDensity
        if (density > 0) {
          const color = this.getDensityColor(density)
          ctx.fillStyle = color
          ctx.fillRect(x * cellSize, z * cellSize, cellSize, cellSize)
        }
      }
    }
    
    // 应用高斯模糊效果
    ctx.filter = 'blur(2px)'
    ctx.globalCompositeOperation = 'multiply'
    ctx.drawImage(canvas, 0, 0)
    ctx.filter = 'none'
    ctx.globalCompositeOperation = 'source-over'
  }

  /// <summary>
  /// 渲染工种热力图
  /// </summary>
  private renderWorktypeHeatmap(personnelList: any[]): void {
    const ctx = this.heatmapContext
    const canvas = this.heatmapCanvas
    
    const professionColors = {
      '钢筋工': 'rgba(255, 107, 53, 0.8)',
      '混凝土工': 'rgba(78, 205, 196, 0.8)',
      '架子工': 'rgba(255, 230, 109, 0.8)',
      '电工': 'rgba(69, 183, 209, 0.8)',
      '安全员': 'rgba(243, 139, 168, 0.8)'
    }
    
    personnelList.forEach(person => {
      const x = ((person.position.x + 100) / 200) * canvas.width
      const z = ((person.position.z + 100) / 200) * canvas.height
      
      const color = professionColors[person.profession as keyof typeof professionColors] || 'rgba(150, 206, 180, 0.8)'
      
      ctx.fillStyle = color
      ctx.beginPath()
      ctx.arc(x, z, 12, 0, Math.PI * 2)
      ctx.fill()
    })
  }

  /// <summary>
  /// 获取密度颜色
  /// </summary>
  private getDensityColor(density: number): string {
    // 密度颜色映射: 绿色(低) -> 黄色 -> 橙色 -> 红色 -> 紫色(高)
    if (density < 0.2) return `rgba(76, 175, 80, ${0.3 + density * 0.7})`
    if (density < 0.4) return `rgba(255, 193, 7, ${0.4 + density * 0.6})`
    if (density < 0.6) return `rgba(255, 152, 0, ${0.5 + density * 0.5})`
    if (density < 0.8) return `rgba(244, 67, 54, ${0.6 + density * 0.4})`
    return `rgba(156, 39, 176, ${0.7 + density * 0.3})`
  }

  /// <summary>
  /// 显示人员轨迹
  /// </summary>
  public displayTrajectory(trajectoryPoints: any[]): void {
    // 清除现有轨迹
    this.trajectoryGroup.clear()
    
    if (!trajectoryPoints || trajectoryPoints.length < 2) return
    
    // 创建轨迹线
    const points = trajectoryPoints.map(point => 
      new THREE.Vector3(point.position.x, point.position.y + 0.5, point.position.z)
    )
    
    const geometry = new THREE.TubeGeometry(
      new THREE.CatmullRomCurve3(points),
      trajectoryPoints.length * 2,
      0.2,
      8,
      false
    )
    
    const material = new THREE.MeshLambertMaterial({ 
      color: 0x00ff88,
      transparent: true,
      opacity: 0.8
    })
    
    this.trajectoryLine = new THREE.Mesh(geometry, material)
    this.trajectoryGroup.add(this.trajectoryLine)
    
    // 创建轨迹点标记
    trajectoryPoints.forEach((point, index) => {
      if (index % 5 === 0) { // 每5个点显示一个标记
        const markerGeometry = new THREE.SphereGeometry(0.3)
        const markerMaterial = new THREE.MeshLambertMaterial({ color: 0xff4444 })
        const marker = new THREE.Mesh(markerGeometry, markerMaterial)
        marker.position.set(point.position.x, point.position.y + 0.5, point.position.z)
        this.trajectoryGroup.add(marker)
      }
    })
  }

  /// <summary>
  /// 播放轨迹动画 - 30fps流畅回放
  /// </summary>
  public playTrajectoryAnimation(
    trajectoryPoints: any[], 
    speed: number = 1,
    onProgress?: (progress: number) => void,
    onComplete?: () => void
  ): void {
    if (!trajectoryPoints || trajectoryPoints.length < 2) return
    
    // 创建移动标记
    if (this.trajectoryMarker) {
      this.trajectoryGroup.remove(this.trajectoryMarker)
    }
    
    const markerGeometry = new THREE.SphereGeometry(1)
    const markerMaterial = new THREE.MeshLambertMaterial({ 
      color: 0xff0000,
      emissive: 0x330000
    })
    this.trajectoryMarker = new THREE.Mesh(markerGeometry, markerMaterial)
    this.trajectoryGroup.add(this.trajectoryMarker)
    
    // 动画参数
    let currentIndex = 0
    const totalPoints = trajectoryPoints.length
    const animationSpeed = speed * 60 // 60fps基准
    
    const animate = () => {
      if (this.isDestroyed || !this.trajectoryMarker) return
      
      const progress = currentIndex / (totalPoints - 1)
      const point = trajectoryPoints[Math.floor(currentIndex)]
      
      if (point) {
        this.trajectoryMarker.position.set(
          point.position.x,
          point.position.y + 2,
          point.position.z
        )
        
        // 添加发光效果
        this.trajectoryMarker.material.emissive.setHex(
          Math.sin(Date.now() * 0.01) > 0 ? 0x660000 : 0x330000
        )
      }
      
      onProgress?.(progress * 100)
      
      currentIndex += animationSpeed / 60
      
      if (currentIndex >= totalPoints - 1) {
        onComplete?.()
        return
      }
      
      this.trajectoryAnimation = requestAnimationFrame(animate)
    }
    
    animate()
  }

  /// <summary>
  /// 暂停轨迹动画
  /// </summary>
  public pauseTrajectoryAnimation(): void {
    if (this.trajectoryAnimation) {
      cancelAnimationFrame(this.trajectoryAnimation)
      this.trajectoryAnimation = null
    }
  }

  /// <summary>
  /// 停止轨迹动画
  /// </summary>
  public stopTrajectoryAnimation(): void {
    this.pauseTrajectoryAnimation()
    
    if (this.trajectoryMarker) {
      this.trajectoryGroup.remove(this.trajectoryMarker)
      this.trajectoryMarker = null
    }
  }

  /// <summary>
  /// 跳转轨迹进度
  /// </summary>
  public seekTrajectoryAnimation(progress: number): void {
    // TODO: 实现轨迹跳转
  }

  /// <summary>
  /// 设置播放速度
  /// </summary>
  public setPlaybackSpeed(speed: number): void {
    // TODO: 实现播放速度调整
  }

  /// <summary>
  /// 聚焦到指定人员
  /// </summary>
  public focusOnPersonnel(person: any): void {
    const targetPosition = new THREE.Vector3(person.position.x, person.position.y + 10, person.position.z + 20)
    const targetLookAt = new THREE.Vector3(person.position.x, person.position.y, person.position.z)
    
    this.animateCamera(targetPosition, targetLookAt)
  }

  /// <summary>
  /// 相机动画
  /// </summary>
  private animateCamera(targetPosition: THREE.Vector3, targetLookAt: THREE.Vector3): void {
    const startPosition = this.camera.position.clone()
    const startTarget = this.controls.target.clone()
    
    let progress = 0
    const duration = 1500
    const startTime = Date.now()
    
    const animate = () => {
      const elapsed = Date.now() - startTime
      progress = Math.min(elapsed / duration, 1)
      
      const eased = this.easeInOutCubic(progress)
      
      this.camera.position.lerpVectors(startPosition, targetPosition, eased)
      this.controls.target.lerpVectors(startTarget, targetLookAt, eased)
      this.controls.update()
      
      if (progress < 1) {
        requestAnimationFrame(animate)
      }
    }
    
    animate()
  }

  /// <summary>
  /// 缓动函数
  /// </summary>
  private easeInOutCubic(t: number): number {
    return t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1
  }

  /// <summary>
  /// 鼠标点击处理
  /// </summary>
  private handleClick(event: MouseEvent): void {
    this.updateMousePosition(event)
    this.raycaster.setFromCamera(this.mouse, this.camera)
    
    // 检测人员点击
    const personnelObjects: THREE.Object3D[] = []
    this.personnelInstances.forEach(instance => {
      personnelObjects.push(instance)
    })
    
    const intersects = this.raycaster.intersectObjects(personnelObjects)
    if (intersects.length > 0) {
      const instancedMesh = intersects[0].object as THREE.InstancedMesh
      const instanceId = intersects[0].instanceId
      const profession = instancedMesh.userData.profession
      const personKey = `${profession}_${instanceId}`
      const person = this.personnelData.get(personKey)
      
      if (person) {
        this.dispatchEvent(new CustomEvent('personnelClick', { detail: person }))
      }
    }
    
    // 检测区域点击
    const areaIntersects = this.raycaster.intersectObjects(Array.from(this.workAreas.values()))
    if (areaIntersects.length > 0) {
      const area = areaIntersects[0].object.userData.area
      this.dispatchEvent(new CustomEvent('areaClick', { detail: area }))
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
    
    const animate = (currentTime: number) => {
      if (this.isDestroyed) return
      
      this.animationId = requestAnimationFrame(animate)
      
      // 性能监控
      this.frameCount++
      if (currentTime - this.lastTime >= 1000) {
        this.performanceStats.fps = Math.round((this.frameCount * 1000) / (currentTime - this.lastTime))
        this.performanceStats.drawCalls = this.renderer.info.render.calls
        this.performanceStats.triangles = this.renderer.info.render.triangles
        this.performanceStats.memory = Math.round((performance as any).memory?.usedJSHeapSize / 1024 / 1024) || 0
        
        this.dispatchEvent(new CustomEvent('performanceUpdate', {
          detail: this.performanceStats
        }))
        
        this.frameCount = 0
        this.lastTime = currentTime
        
        // 重置渲染信息
        this.renderer.info.reset()
      }
      
      // 更新控制器
      this.controls.update()
      
      // 渲染场景
      this.renderer.render(this.scene, this.camera)
    }
    
    animate(0)
  }

  /// <summary>
  /// 销毁引擎
  /// </summary>
  public destroy(): void {
    this.isDestroyed = true
    
    if (this.animationId) {
      cancelAnimationFrame(this.animationId)
    }
    
    if (this.trajectoryAnimation) {
      cancelAnimationFrame(this.trajectoryAnimation)
    }
    
    // 清理事件监听器
    window.removeEventListener('resize', this.handleResize.bind(this))
    
    // 清理Three.js对象
    this.scene.clear()
    this.renderer.dispose()
    
    // 清理DOM
    if (this.container.contains(this.renderer.domElement)) {
      this.container.removeChild(this.renderer.domElement)
    }
    
    // 清理纹理
    this.heatmapTexture.dispose()
  }
} 