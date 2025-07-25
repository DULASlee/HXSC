import * as THREE from 'three'
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js'
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader.js'


/**
 * 环境监测3D可视化引擎
 * 支持污染扩散模拟、智能联动可视化、声纹热力图、历史数据对比
 * 核心算法: 高斯扩散模型、PM2.5粒子系统、声纹球面可视化
 */
export class EnvironmentVisualizationEngine extends EventTarget {
  private container: HTMLElement
  private scene: THREE.Scene
  private camera: THREE.PerspectiveCamera
  private renderer: THREE.WebGLRenderer
  private controls: OrbitControls
  private loader: GLTFLoader
  
  // 监测站点相关
  private stationsGroup: THREE.Group = new THREE.Group()
  private stationModels: Map<string, THREE.Group> = new Map()
  private stationData: Map<string, any> = new Map()
  
  // 污染扩散相关
  private diffusionGroup: THREE.Group = new THREE.Group()
  private particleSystems: Map<string, THREE.Points> = new Map()
  private diffusionClouds: Map<string, THREE.Mesh> = new Map()
  private diffusionEnabled: boolean = false
  private diffusionParams: any = {}
  
  // 声纹可视化相关
  private soundGroup: THREE.Group = new THREE.Group()
  private soundWaves: Map<string, THREE.Mesh[]> = new Map()
  private soundHeatmaps: Map<string, THREE.Mesh> = new Map()
  
  // 智能联动相关
  private linkageGroup: THREE.Group = new THREE.Group()
  private treatmentDevices: Map<string, THREE.Group> = new Map()
  private treatmentEffects: Map<string, THREE.Points> = new Map()
  
  // 历史数据相关
  private historyGroup: THREE.Group = new THREE.Group()
  private historyMarkers: THREE.Group = new THREE.Group()
  private isShowingHistory: boolean = false
  
  // 环境数据
  private windField: THREE.Vector3 = new THREE.Vector3(1, 0, 0.5)
  private temperatureMap: Map<string, number> = new Map()
  private humidityMap: Map<string, number> = new Map()
  
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
    this.scene.background = new THREE.Color(0x0f172a)
    this.scene.fog = new THREE.Fog(0x0f172a, 100, 800)
    
    // 添加组
    this.scene.add(this.stationsGroup)
    this.scene.add(this.diffusionGroup)
    this.scene.add(this.soundGroup)
    this.scene.add(this.linkageGroup)
    this.scene.add(this.historyGroup)
    this.scene.add(this.historyMarkers)
    
    // 设置组名称
    this.stationsGroup.name = 'MonitoringStations'
    this.diffusionGroup.name = 'PollutionDiffusion'
    this.soundGroup.name = 'SoundVisualization'
    this.linkageGroup.name = 'SmartLinkage'
    this.historyGroup.name = 'HistoryData'
  }

  /// <summary>
  /// 初始化相机
  /// </summary>
  private initCamera(): void {
    const aspect = this.container.clientWidth / this.container.clientHeight
    this.camera = new THREE.PerspectiveCamera(50, aspect, 1, 2000)
    this.camera.position.set(150, 120, 200)
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
    this.renderer.toneMappingExposure = 1.0
    
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
    this.controls.minDistance = 50
    this.controls.maxDistance = 600
    this.controls.autoRotate = false
    this.controls.autoRotateSpeed = 1.0
  }

  /// <summary>
  /// 初始化灯光
  /// </summary>
  private initLights(): void {
    // 环境光
    const ambientLight = new THREE.AmbientLight(0x404040, 0.6)
    this.scene.add(ambientLight)
    
    // 主方向光 - 模拟日光
    const sunLight = new THREE.DirectionalLight(0xffffff, 1.2)
    sunLight.position.set(100, 150, 50)
    sunLight.castShadow = true
    sunLight.shadow.mapSize.width = 2048
    sunLight.shadow.mapSize.height = 2048
    sunLight.shadow.camera.near = 0.5
    sunLight.shadow.camera.far = 500
    sunLight.shadow.camera.left = -150
    sunLight.shadow.camera.right = 150
    sunLight.shadow.camera.top = 150
    sunLight.shadow.camera.bottom = -150
    this.scene.add(sunLight)
    
    // 补充光源 - 模拟天空光
    const skyLight = new THREE.DirectionalLight(0x87ceeb, 0.4)
    skyLight.position.set(-50, 100, -100)
    this.scene.add(skyLight)
    
    // 点光源 - 增强局部照明
    const pointLight = new THREE.PointLight(0xffffff, 0.5, 200)
    pointLight.position.set(0, 80, 0)
    this.scene.add(pointLight)
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
  /// 初始化引擎
  /// </summary>
  public async init(): Promise<void> {
    // 创建地面
    this.createGround()
    
    // 创建建筑物
    this.createBuildings()
    
    // 创建天空盒
    this.createSkybox()
    
    // 初始化动画系统
  }

  /// <summary>
  /// 创建地面
  /// </summary>
  private createGround(): void {
    const groundGeometry = new THREE.PlaneGeometry(500, 500, 100, 100)
    const groundMaterial = new THREE.MeshLambertMaterial({ 
      color: 0x3a4f3a,
      transparent: true,
      opacity: 0.8
    })
    
    // 添加地面高度变化
    const vertices = groundGeometry.attributes.position.array as Float32Array
    for (let i = 0; i < vertices.length; i += 3) {
      vertices[i + 2] = Math.random() * 2 - 1 // 随机高度变化
    }
    groundGeometry.attributes.position.needsUpdate = true
    groundGeometry.computeVertexNormals()
    
    const ground = new THREE.Mesh(groundGeometry, groundMaterial)
    ground.rotation.x = -Math.PI / 2
    ground.receiveShadow = true
    this.scene.add(ground)
    
    // 地面网格
    const gridHelper = new THREE.GridHelper(500, 50, 0x444444, 0x222222)
    gridHelper.material.transparent = true
    gridHelper.material.opacity = 0.2
    this.scene.add(gridHelper)
  }

  /// <summary>
  /// 创建建筑物
  /// </summary>
  private createBuildings(): void {
    const buildings = [
      { position: [0, 15, 0], size: [40, 30, 20], color: 0x8B7355 },
      { position: [70, 20, 30], size: [25, 40, 15], color: 0x696969 },
      { position: [-60, 12, -25], size: [35, 24, 18], color: 0x8B4513 },
      { position: [35, 16, -70], size: [30, 32, 16], color: 0x708090 },
      { position: [-80, 18, 40], size: [28, 36, 20], color: 0x9ACD32 }
    ]
    
    buildings.forEach((building, index) => {
      const geometry = new THREE.BoxGeometry(building.size[0], building.size[1], building.size[2])
      const material = new THREE.MeshLambertMaterial({ 
        color: building.color,
        transparent: true,
        opacity: 0.9
      })
      const mesh = new THREE.Mesh(geometry, material)
      mesh.position.set(building.position[0], building.position[1], building.position[2])
      mesh.castShadow = true
      mesh.receiveShadow = true
      this.scene.add(mesh)
    })
  }

  /// <summary>
  /// 创建天空盒
  /// </summary>
  private createSkybox(): void {
    const skyGeometry = new THREE.SphereGeometry(1000, 32, 16)
    const skyMaterial = new THREE.MeshBasicMaterial({
      color: 0x87ceeb,
      side: THREE.BackSide,
      transparent: true,
      opacity: 0.3
    })
    const sky = new THREE.Mesh(skyGeometry, skyMaterial)
    this.scene.add(sky)
  }

  /// <summary>
  /// 添加监测站点
  /// </summary>
  public addMonitoringStation(stationData: any): void {
    try {
      const stationGroup = this.createStationModel(stationData)
      
      this.stationModels.set(stationData.id, stationGroup)
      this.stationData.set(stationData.id, stationData)
      this.stationsGroup.add(stationGroup)
      
      // 创建声纹可视化
      this.createSoundVisualization(stationData)
      
      // 如果站点超标，创建污染源
      if (stationData.isExceeding && this.diffusionEnabled) {
        this.createPollutionSource(stationData)
      }
      
      console.log(`监测站点 ${stationData.name} 添加成功`)
      
    } catch (error) {
      console.error(`添加监测站点失败: ${stationData.id}`, error)
    }
  }

  /// <summary>
  /// 创建监测站点模型
  /// </summary>
  private createStationModel(stationData: any): THREE.Group {
    const stationGroup = new THREE.Group()
    stationGroup.name = stationData.id
    
    // 基座
    const baseGeometry = new THREE.CylinderGeometry(3, 3, 1, 8)
    const baseMaterial = new THREE.MeshLambertMaterial({ 
      color: stationData.status === 'online' ? 0x4a90e2 : 0x666666 
    })
    const base = new THREE.Mesh(baseGeometry, baseMaterial)
    base.position.y = 0.5
    base.receiveShadow = true
    
    // 主柱
    const poleGeometry = new THREE.CylinderGeometry(0.3, 0.3, 8, 8)
    const poleMaterial = new THREE.MeshLambertMaterial({ color: 0xcccccc })
    const pole = new THREE.Mesh(poleGeometry, poleMaterial)
    pole.position.y = 5
    pole.castShadow = true
    
    // 传感器盒子
    const sensorGeometry = new THREE.BoxGeometry(2, 1.5, 1)
    const sensorMaterial = new THREE.MeshLambertMaterial({ 
      color: stationData.isExceeding ? 0xff4444 : 0x44ff44 
    })
    const sensorBox = new THREE.Mesh(sensorGeometry, sensorMaterial)
    sensorBox.position.y = 9.5
    sensorBox.castShadow = true
    sensorBox.name = 'sensor'
    
    // 状态指示灯
    const lightGeometry = new THREE.SphereGeometry(0.2)
    const lightMaterial = new THREE.MeshBasicMaterial({ 
      color: stationData.status === 'online' ? 0x00ff00 : 0xff0000,
      emissive: stationData.status === 'online' ? 0x004400 : 0x440000
    })
    const statusLight = new THREE.Mesh(lightGeometry, lightMaterial)
    statusLight.position.set(0, 10.5, 0.7)
    statusLight.name = 'statusLight'
    
    stationGroup.add(base)
    stationGroup.add(pole)
    stationGroup.add(sensorBox)
    stationGroup.add(statusLight)
    
    // 设置位置
    stationGroup.position.set(
      stationData.position.x,
      stationData.position.y || 0,
      stationData.position.z
    )
    
    stationGroup.userData = { stationData }
    
    return stationGroup
  }

  /// <summary>
  /// 创建声纹可视化
  /// </summary>
  private createSoundVisualization(stationData: any): void {
    if (stationData.noise <= 0) return
    
    const soundWaves: THREE.Mesh[] = []
    const centerPosition = new THREE.Vector3(
      stationData.position.x,
      stationData.position.y + 5,
      stationData.position.z
    )
    
    // 创建多层声波球面
    for (let i = 1; i <= 3; i++) {
      const radius = (stationData.noise / 70) * 15 * i
      const geometry = new THREE.SphereGeometry(radius, 16, 8)
      
      // 根据噪音等级设置颜色
      let color = 0x00ff00
      if (stationData.noise > 70) color = 0xff0000
      else if (stationData.noise > 55) color = 0xffaa00
      
      const material = new THREE.MeshBasicMaterial({
        color,
        transparent: true,
        opacity: 0.1 / i,
        side: THREE.DoubleSide,
        wireframe: true
      })
      
      const sphere = new THREE.Mesh(geometry, material)
      sphere.position.copy(centerPosition)
      
      soundWaves.push(sphere)
      this.soundGroup.add(sphere)
      
      // 添加脉冲动画
      this.animateSoundWave(sphere, i * 500)
    }
    
    this.soundWaves.set(stationData.id, soundWaves)
  }

  /// <summary>
  /// 动画声波
  /// </summary>
  private animateSoundWave(sphere: THREE.Mesh, delay: number): void {
    const originalScale = sphere.scale.clone()
    
    // 简单的缩放动画
    const animate = () => {
      if (this.isDestroyed) return
      
      sphere.scale.setScalar(originalScale.x * 1.2)
      setTimeout(() => {
        sphere.scale.setScalar(originalScale.x)
      }, 1000)
    }
    
    animate()
  }

  /// <summary>
  /// 创建污染源
  /// </summary>
  private createPollutionSource(stationData: any): void {
    // 创建粒子系统模拟PM2.5扩散
    const particleCount = Math.min(stationData.pm25 * 10, 2000)
    const geometry = new THREE.BufferGeometry()
    const positions = new Float32Array(particleCount * 3)
    const colors = new Float32Array(particleCount * 3)
    const velocities = new Float32Array(particleCount * 3)
    
    const sourcePosition = new THREE.Vector3(
      stationData.position.x,
      stationData.position.y + 2,
      stationData.position.z
    )
    
    for (let i = 0; i < particleCount; i++) {
      const i3 = i * 3
      
      // 初始位置在污染源附近
      positions[i3] = sourcePosition.x + (Math.random() - 0.5) * 5
      positions[i3 + 1] = sourcePosition.y + Math.random() * 3
      positions[i3 + 2] = sourcePosition.z + (Math.random() - 0.5) * 5
      
      // 粒子颜色 - 根据PM2.5浓度
      const intensity = Math.min(stationData.pm25 / 100, 1)
      colors[i3] = 0.8 + intensity * 0.2     // Red
      colors[i3 + 1] = 0.6 - intensity * 0.4 // Green  
      colors[i3 + 2] = 0.2 - intensity * 0.2 // Blue
      
      // 初始速度
      velocities[i3] = (Math.random() - 0.5) * 0.2 + this.windField.x * 0.1
      velocities[i3 + 1] = Math.random() * 0.1
      velocities[i3 + 2] = (Math.random() - 0.5) * 0.2 + this.windField.z * 0.1
    }
    
    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3))
    geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3))
    geometry.setAttribute('velocity', new THREE.BufferAttribute(velocities, 3))
    
    const material = new THREE.PointsMaterial({
      size: 0.5,
      vertexColors: true,
      transparent: true,
      opacity: 0.7,
      blending: THREE.AdditiveBlending
    })
    
    const particles = new THREE.Points(geometry, material)
    particles.userData = { 
      sourcePosition: sourcePosition.clone(),
      stationId: stationData.id,
      type: 'pollution'
    }
    
    this.particleSystems.set(stationData.id, particles)
    this.diffusionGroup.add(particles)
  }

  /// <summary>
  /// 启用扩散模拟
  /// </summary>
  public enableDiffusionSimulation(enabled: boolean): void {
    this.diffusionEnabled = enabled
    this.diffusionGroup.visible = enabled
    
    if (enabled) {
      // 为所有超标站点创建污染源
      this.stationData.forEach(station => {
        if (station.isExceeding && !this.particleSystems.has(station.id)) {
          this.createPollutionSource(station)
        }
      })
    } else {
      // 清除所有粒子系统
      this.particleSystems.clear()
      this.diffusionGroup.clear()
    }
  }

  /// <summary>
  /// 更新扩散参数
  /// </summary>
  public updateDiffusionParameters(params: any): void {
    this.diffusionParams = params
    
    // 更新风场
    const windSpeed = params.windInfluence * 5
    this.windField.set(windSpeed, 0, windSpeed * 0.5)
    
    // 更新现有粒子系统
    this.particleSystems.forEach((particles, stationId) => {
      this.updateParticleSystem(particles, params)
    })
    
    // 创建扩散云团
    this.createDiffusionClouds(params)
  }

  /// <summary>
  /// 更新粒子系统
  /// </summary>
  private updateParticleSystem(particles: THREE.Points, params: any): void {
    const geometry = particles.geometry
    const velocities = geometry.getAttribute('velocity') as THREE.BufferAttribute
    
    // 根据风速和稳定度调整粒子速度
    const windFactor = params.windInfluence
    const stabilityFactor = this.getStabilityFactor(params.stability)
    
    for (let i = 0; i < velocities.count; i++) {
      const i3 = i * 3
      velocities.array[i3] *= windFactor * stabilityFactor
      velocities.array[i3 + 2] *= windFactor * stabilityFactor
    }
    
    velocities.needsUpdate = true
  }

  /// <summary>
  /// 创建扩散云团
  /// </summary>
  private createDiffusionClouds(params: any): void {
    // 清除现有云团
    this.diffusionClouds.forEach(cloud => {
      this.diffusionGroup.remove(cloud)
    })
    this.diffusionClouds.clear()
    
    // 为每个污染源创建扩散云团
    params.sources.forEach((source: any) => {
      this.createGaussianDiffusionCloud(source, params)
    })
  }

  /// <summary>
  /// 创建高斯扩散云团
  /// </summary>
  private createGaussianDiffusionCloud(source: any, params: any): void {
    const cloudGeometry = new THREE.SphereGeometry(50, 16, 8)
    const cloudMaterial = new THREE.MeshBasicMaterial({
      color: 0xff6b35,
      transparent: true,
      opacity: 0.15,
      side: THREE.DoubleSide
    })
    
    const cloud = new THREE.Mesh(cloudGeometry, cloudMaterial)
    cloud.position.set(source.position.x, source.position.y + 10, source.position.z)
    
    // 根据高斯扩散模型调整云团形状
    const windDirection = this.windField.normalize()
    const stabilityFactor = this.getStabilityFactor(params.stability)
    
    // 扩散云团椭球化 - 顺风向拉长
    cloud.scale.set(
      1 + Math.abs(windDirection.x) * 2,
      stabilityFactor,
      1 + Math.abs(windDirection.z) * 2
    )
    
    this.diffusionClouds.set(source.id, cloud)
    this.diffusionGroup.add(cloud)
    
    // 添加飘移动画
    this.animateCloudDrift(cloud, windDirection)
  }

  /// <summary>
  /// 动画云团飘移
  /// </summary>
  private animateCloudDrift(cloud: THREE.Mesh, windDirection: THREE.Vector3): void {
    const originalPosition = cloud.position.clone()
    const driftDistance = 30
    
    const driftTarget = originalPosition.clone().add(
      windDirection.clone().multiplyScalar(driftDistance)
    )
    
    new TWEEN.Tween(cloud.position)
      .to(driftTarget, 10000)
      .easing(TWEEN.Easing.Linear.None)
      .repeat(Infinity)
      .yoyo(true)
      .start()
  }

  /// <summary>
  /// 获取稳定度系数
  /// </summary>
  private getStabilityFactor(stability: string): number {
    const factors = { 
      A: 1.8, // 极不稳定 - 垂直扩散强
      B: 1.5, // 不稳定
      C: 1.2, // 轻微不稳定
      D: 1.0, // 中性
      E: 0.7, // 轻微稳定
      F: 0.4  // 稳定 - 垂直扩散弱
    }
    return factors[stability as keyof typeof factors] || 1.0
  }

  /// <summary>
  /// 高亮站点
  /// </summary>
  public highlightStation(stationId: string): void {
    // 重置所有站点高亮
    this.stationModels.forEach((group, id) => {
      const sensor = group.getObjectByName('sensor') as THREE.Mesh
      if (sensor) {
        const material = sensor.material as THREE.MeshLambertMaterial
        material.emissive.setHex(0x000000)
      }
    })
    
    // 高亮选中的站点
    const stationGroup = this.stationModels.get(stationId)
    if (stationGroup) {
      const sensor = stationGroup.getObjectByName('sensor') as THREE.Mesh
      if (sensor) {
        const material = sensor.material as THREE.MeshLambertMaterial
        material.emissive.setHex(0x444444)
      }
      
      // 相机焦点移动到站点
      this.focusOnStation(stationId)
    }
  }

  /// <summary>
  /// 聚焦到站点
  /// </summary>
  public focusOnStation(stationId: string): void {
    const stationGroup = this.stationModels.get(stationId)
    if (!stationGroup) return
    
    const targetPosition = stationGroup.position.clone()
    targetPosition.y += 30
    targetPosition.z += 50
    
    const targetLookAt = stationGroup.position.clone()
    targetLookAt.y += 10
    
    this.animateCamera(targetPosition, targetLookAt)
  }

  /// <summary>
  /// 相机动画
  /// </summary>
  private animateCamera(targetPosition: THREE.Vector3, targetLookAt: THREE.Vector3): void {
    new TWEEN.Tween(this.camera.position)
      .to(targetPosition, 2000)
      .easing(TWEEN.Easing.Cubic.InOut)
      .start()
    
    new TWEEN.Tween(this.controls.target)
      .to(targetLookAt, 2000)
      .easing(TWEEN.Easing.Cubic.InOut)
      .onUpdate(() => {
        this.controls.update()
      })
      .start()
  }

  /// <summary>
  /// 更新站点数据
  /// </summary>
  public updateStationData(stationId: string, newData: any): void {
    const stationData = this.stationData.get(stationId)
    if (!stationData) return
    
    // 更新数据
    Object.assign(stationData, newData)
    this.stationData.set(stationId, stationData)
    
    // 更新3D模型
    this.updateStationVisuals(stationId, stationData)
  }

  /// <summary>
  /// 更新站点视觉效果
  /// </summary>
  private updateStationVisuals(stationId: string, stationData: any): void {
    const stationGroup = this.stationModels.get(stationId)
    if (!stationGroup) return
    
    // 更新传感器盒子颜色
    const sensor = stationGroup.getObjectByName('sensor') as THREE.Mesh
    if (sensor) {
      const material = sensor.material as THREE.MeshLambertMaterial
      material.color.setHex(stationData.isExceeding ? 0xff4444 : 0x44ff44)
    }
    
    // 更新状态指示灯
    const statusLight = stationGroup.getObjectByName('statusLight') as THREE.Mesh
    if (statusLight) {
      const material = statusLight.material as THREE.MeshBasicMaterial
      if (stationData.status === 'online') {
        material.color.setHex(0x00ff00)
        material.emissive.setHex(0x004400)
      } else {
        material.color.setHex(0xff0000)
        material.emissive.setHex(0x440000)
      }
    }
    
    // 更新声纹可视化
    this.updateSoundVisualization(stationId, stationData)
    
    // 更新污染源
    if (stationData.isExceeding && this.diffusionEnabled) {
      if (!this.particleSystems.has(stationId)) {
        this.createPollutionSource(stationData)
      }
    } else {
      // 移除污染源
      const particles = this.particleSystems.get(stationId)
      if (particles) {
        this.diffusionGroup.remove(particles)
        this.particleSystems.delete(stationId)
      }
    }
  }

  /// <summary>
  /// 更新声纹可视化
  /// </summary>
  private updateSoundVisualization(stationId: string, stationData: any): void {
    const soundWaves = this.soundWaves.get(stationId)
    if (!soundWaves) return
    
    soundWaves.forEach((wave, index) => {
      const baseRadius = (stationData.noise / 70) * 15
      const radius = baseRadius * (index + 1)
      
      // 更新半径
      wave.scale.setScalar(radius / 15)
      
      // 更新颜色
      let color = 0x00ff00
      if (stationData.noise > 70) color = 0xff0000
      else if (stationData.noise > 55) color = 0xffaa00
      
      const material = wave.material as THREE.MeshBasicMaterial
      material.color.setHex(color)
      
      // 更新透明度
      material.opacity = (0.1 / (index + 1)) * (stationData.noise / 100)
    })
  }

  /// <summary>
  /// 激活应急治理
  /// </summary>
  public activateEmergencyTreatment(): void {
    // 创建雾炮车效果
    this.createFogCannonEffect()
    
    // 创建喷淋系统效果
    this.createSprinklerEffect()
    
    // 启动净化效果动画
    this.startPurificationAnimation()
  }

  /// <summary>
  /// 创建雾炮车效果
  /// </summary>
  private createFogCannonEffect(): void {
    const positions = [
      { x: 50, z: 30 },
      { x: -40, z: -20 },
      { x: 20, z: -60 }
    ]
    
    positions.forEach((pos, index) => {
      // 雾炮车模型
      const vehicleGroup = new THREE.Group()
      
      // 车身
      const bodyGeometry = new THREE.BoxGeometry(8, 3, 4)
      const bodyMaterial = new THREE.MeshLambertMaterial({ color: 0x4a90e2 })
      const body = new THREE.Mesh(bodyGeometry, bodyMaterial)
      body.position.y = 1.5
      body.castShadow = true
      
      // 雾炮
      const cannonGeometry = new THREE.CylinderGeometry(0.5, 0.8, 4)
      const cannonMaterial = new THREE.MeshLambertMaterial({ color: 0x666666 })
      const cannon = new THREE.Mesh(cannonGeometry, cannonMaterial)
      cannon.position.set(0, 4, 0)
      cannon.rotation.z = Math.PI / 6
      cannon.castShadow = true
      
      vehicleGroup.add(body)
      vehicleGroup.add(cannon)
      vehicleGroup.position.set(pos.x, 0, pos.z)
      
      this.treatmentDevices.set(`fogcannon_${index}`, vehicleGroup)
      this.linkageGroup.add(vehicleGroup)
      
      // 创建喷雾效果
      this.createMistEffect(vehicleGroup.position)
    })
  }

  /// <summary>
  /// 创建喷雾效果
  /// </summary>
  private createMistEffect(position: THREE.Vector3): void {
    const particleCount = 1000
    const geometry = new THREE.BufferGeometry()
    const positions = new Float32Array(particleCount * 3)
    const colors = new Float32Array(particleCount * 3)
    
    for (let i = 0; i < particleCount; i++) {
      const i3 = i * 3
      
      // 喷雾粒子位置
      positions[i3] = position.x + (Math.random() - 0.5) * 20
      positions[i3 + 1] = position.y + Math.random() * 10 + 3
      positions[i3 + 2] = position.z + (Math.random() - 0.5) * 20
      
      // 白色雾气
      colors[i3] = 0.9 + Math.random() * 0.1
      colors[i3 + 1] = 0.9 + Math.random() * 0.1
      colors[i3 + 2] = 1.0
    }
    
    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3))
    geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3))
    
    const material = new THREE.PointsMaterial({
      size: 1.0,
      vertexColors: true,
      transparent: true,
      opacity: 0.6,
      blending: THREE.AdditiveBlending
    })
    
    const mist = new THREE.Points(geometry, material)
    mist.userData = { type: 'mist' }
    
    this.treatmentEffects.set(`mist_${position.x}_${position.z}`, mist)
    this.linkageGroup.add(mist)
  }

  /// <summary>
  /// 创建喷淋系统效果
  /// </summary>
  private createSprinklerEffect(): void {
    const gridSize = 5
    const spacing = 20
    
    for (let x = -2; x <= 2; x++) {
      for (let z = -2; z <= 2; z++) {
        const position = new THREE.Vector3(x * spacing, 0, z * spacing)
        
        // 喷淋头
        const sprinklerGeometry = new THREE.CylinderGeometry(0.3, 0.3, 6)
        const sprinklerMaterial = new THREE.MeshLambertMaterial({ color: 0x888888 })
        const sprinkler = new THREE.Mesh(sprinklerGeometry, sprinklerMaterial)
        sprinkler.position.copy(position)
        sprinkler.position.y = 8
        sprinkler.castShadow = true
        
        this.linkageGroup.add(sprinkler)
        
        // 水滴效果
        this.createWaterDropEffect(sprinkler.position)
      }
    }
  }

  /// <summary>
  /// 创建水滴效果
  /// </summary>
  private createWaterDropEffect(position: THREE.Vector3): void {
    const dropCount = 500
    const geometry = new THREE.BufferGeometry()
    const positions = new Float32Array(dropCount * 3)
    const colors = new Float32Array(dropCount * 3)
    
    for (let i = 0; i < dropCount; i++) {
      const i3 = i * 3
      
      // 水滴初始位置
      positions[i3] = position.x + (Math.random() - 0.5) * 10
      positions[i3 + 1] = position.y - Math.random() * 8
      positions[i3 + 2] = position.z + (Math.random() - 0.5) * 10
      
      // 蓝色水滴
      colors[i3] = 0.3
      colors[i3 + 1] = 0.6 + Math.random() * 0.3
      colors[i3 + 2] = 1.0
    }
    
    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3))
    geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3))
    
    const material = new THREE.PointsMaterial({
      size: 0.8,
      vertexColors: true,
      transparent: true,
      opacity: 0.8,
      blending: THREE.AdditiveBlending
    })
    
    const drops = new THREE.Points(geometry, material)
    drops.userData = { type: 'waterDrops' }
    
    this.treatmentEffects.set(`drops_${position.x}_${position.z}`, drops)
    this.linkageGroup.add(drops)
  }

  /// <summary>
  /// 启动净化效果动画
  /// </summary>
  private startPurificationAnimation(): void {
    // 逐渐减少污染粒子
    this.particleSystems.forEach(particles => {
      const geometry = particles.geometry
      const positions = geometry.getAttribute('position') as THREE.BufferAttribute
      const colors = geometry.getAttribute('color') as THREE.BufferAttribute
      
      // 动画减少粒子数量和改变颜色
      const animate = () => {
        for (let i = 0; i < colors.count; i++) {
          const i3 = i * 3
          // 逐渐变绿表示净化
          colors.array[i3] = Math.max(0, colors.array[i3] - 0.01) // 减少红色
          colors.array[i3 + 1] = Math.min(1, colors.array[i3 + 1] + 0.02) // 增加绿色
        }
        colors.needsUpdate = true
        
        if (!this.isDestroyed) {
          requestAnimationFrame(animate)
        }
      }
      
      animate()
    })
  }

  /// <summary>
  /// 显示历史数据
  /// </summary>
  public showHistoryData(historyData: any[]): void {
    this.isShowingHistory = true
    
    // 清除现有历史标记
    this.historyMarkers.clear()
    
    // 为每个历史数据点创建标记
    historyData.forEach((data, index) => {
      const stationGroup = this.stationModels.get(data.id)
      if (!stationGroup) return
      
      // 创建历史数据标记
      const markerGeometry = new THREE.SphereGeometry(1)
      const markerMaterial = new THREE.MeshBasicMaterial({
        color: this.getHistoryColor(data.pm25),
        transparent: true,
        opacity: 0.7
      })
      
      const marker = new THREE.Mesh(markerGeometry, markerMaterial)
      marker.position.copy(stationGroup.position)
      marker.position.y += 15
      
      this.historyMarkers.add(marker)
      
      // 添加上下浮动动画
      new TWEEN.Tween(marker.position)
        .to({ y: marker.position.y + 2 }, 1000)
        .easing(TWEEN.Easing.Sinusoidal.InOut)
        .yoyo(true)
        .repeat(Infinity)
        .delay(index * 100)
        .start()
    })
    
    this.historyGroup.add(this.historyMarkers)
  }

  /// <summary>
  /// 获取历史数据颜色
  /// </summary>
  private getHistoryColor(pm25: number): number {
    if (pm25 > 75) return 0xff0000
    if (pm25 > 35) return 0xffaa00
    return 0x00ff00
  }

  /// <summary>
  /// 显示当前数据
  /// </summary>
  public showCurrentData(): void {
    this.isShowingHistory = false
    this.historyGroup.remove(this.historyMarkers)
  }

  /// <summary>
  /// 鼠标点击处理
  /// </summary>
  private handleClick(event: MouseEvent): void {
    this.updateMousePosition(event)
    this.raycaster.setFromCamera(this.mouse, this.camera)
    
    const stationObjects: THREE.Object3D[] = []
    this.stationModels.forEach(group => {
      stationObjects.push(group)
    })
    
    const intersects = this.raycaster.intersectObjects(stationObjects, true)
    if (intersects.length > 0) {
      let stationGroup = intersects[0].object
      while (stationGroup.parent && stationGroup.parent !== this.stationsGroup) {
        stationGroup = stationGroup.parent
      }
      
      const stationData = stationGroup.userData?.stationData
      if (stationData) {
        this.dispatchEvent(new CustomEvent('stationClick', { detail: stationData }))
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
      
      // 更新TWEEN动画
      TWEEN.update()
      
      // 更新粒子系统动画
      this.updateParticleAnimations()
      
      // 渲染场景
      this.renderer.render(this.scene, this.camera)
    }
    
    animate()
  }

  /// <summary>
  /// 更新粒子动画
  /// </summary>
  private updateParticleAnimations(): void {
    this.particleSystems.forEach(particles => {
      this.updateParticlePositions(particles)
    })
    
    this.treatmentEffects.forEach(effect => {
      this.updateTreatmentEffect(effect)
    })
  }

  /// <summary>
  /// 更新粒子位置
  /// </summary>
  private updateParticlePositions(particles: THREE.Points): void {
    const geometry = particles.geometry
    const positions = geometry.getAttribute('position') as THREE.BufferAttribute
    const velocities = geometry.getAttribute('velocity') as THREE.BufferAttribute
    const sourcePosition = particles.userData.sourcePosition
    
    for (let i = 0; i < positions.count; i++) {
      const i3 = i * 3
      
      // 更新位置
      positions.array[i3] += velocities.array[i3]
      positions.array[i3 + 1] += velocities.array[i3 + 1]
      positions.array[i3 + 2] += velocities.array[i3 + 2]
      
      // 重力影响
      velocities.array[i3 + 1] -= 0.001
      
      // 风场影响
      velocities.array[i3] += this.windField.x * 0.01
      velocities.array[i3 + 2] += this.windField.z * 0.01
      
      // 边界检查 - 重置超出范围的粒子
      const distance = Math.sqrt(
        Math.pow(positions.array[i3] - sourcePosition.x, 2) +
        Math.pow(positions.array[i3 + 2] - sourcePosition.z, 2)
      )
      
      if (distance > 100 || positions.array[i3 + 1] < 0) {
        // 重置到源点附近
        positions.array[i3] = sourcePosition.x + (Math.random() - 0.5) * 5
        positions.array[i3 + 1] = sourcePosition.y + Math.random() * 3
        positions.array[i3 + 2] = sourcePosition.z + (Math.random() - 0.5) * 5
        
        velocities.array[i3] = (Math.random() - 0.5) * 0.2 + this.windField.x * 0.1
        velocities.array[i3 + 1] = Math.random() * 0.1
        velocities.array[i3 + 2] = (Math.random() - 0.5) * 0.2 + this.windField.z * 0.1
      }
    }
    
    positions.needsUpdate = true
  }

  /// <summary>
  /// 更新治理效果
  /// </summary>
  private updateTreatmentEffect(effect: THREE.Points): void {
    const geometry = effect.geometry
    const positions = geometry.getAttribute('position') as THREE.BufferAttribute
    
    if (effect.userData.type === 'mist') {
      // 雾气上升和扩散
      for (let i = 0; i < positions.count; i++) {
        const i3 = i * 3
        positions.array[i3 + 1] += 0.05 // 上升
        positions.array[i3] += (Math.random() - 0.5) * 0.02 // 水平扩散
        positions.array[i3 + 2] += (Math.random() - 0.5) * 0.02
        
        // 重置超出高度的粒子
        if (positions.array[i3 + 1] > 20) {
          positions.array[i3 + 1] = 3 + Math.random() * 2
        }
      }
    } else if (effect.userData.type === 'waterDrops') {
      // 水滴下落
      for (let i = 0; i < positions.count; i++) {
        const i3 = i * 3
        positions.array[i3 + 1] -= 0.1 // 下落
        
        // 重置落地的水滴
        if (positions.array[i3 + 1] < 0) {
          positions.array[i3 + 1] = 8 + Math.random() * 2
        }
      }
    }
    
    positions.needsUpdate = true
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
    TWEEN.removeAll()
    
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