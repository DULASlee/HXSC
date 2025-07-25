/**
 * 视频安防引擎
 * 负责3D可视化、WebRTC连接、AI分析等功能
 */
import * as THREE from 'three'

export class VideoSecurityEngine {
  private container: HTMLElement
  private scene: THREE.Scene
  private camera: THREE.PerspectiveCamera
  private renderer: THREE.WebGLRenderer
  private controls: any
  private cameras: Map<string, any> = new Map()
  private eventListeners: Map<string, Function[]> = new Map()
  private animationId: number | null = null

  constructor(container: HTMLElement) {
    this.container = container
    this.scene = new THREE.Scene()
    this.camera = new THREE.PerspectiveCamera(
      75,
      container.clientWidth / container.clientHeight,
      0.1,
      1000
    )
    this.renderer = new THREE.WebGLRenderer({ antialias: true })
  }

  /**
   * 初始化引擎
   */
  async init(): Promise<void> {
    try {
      // 设置渲染器
      this.renderer.setSize(this.container.clientWidth, this.container.clientHeight)
      this.renderer.setClearColor(0x0a0a0a)
      this.container.appendChild(this.renderer.domElement)

      // 设置相机位置
      this.camera.position.set(0, 50, 100)
      this.camera.lookAt(0, 0, 0)

      // 添加环境光
      const ambientLight = new THREE.AmbientLight(0x404040, 0.6)
      this.scene.add(ambientLight)

      // 添加方向光
      const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8)
      directionalLight.position.set(50, 50, 50)
      this.scene.add(directionalLight)

      // 创建基础场景
      this.createBaseScene()

      console.log('VideoSecurityEngine 初始化成功')
    } catch (error) {
      console.error('VideoSecurityEngine 初始化失败:', error)
      throw error
    }
  }

  /**
   * 创建基础场景
   */
  private createBaseScene(): void {
    // 创建地面
    const groundGeometry = new THREE.PlaneGeometry(200, 200)
    const groundMaterial = new THREE.MeshLambertMaterial({ 
      color: 0x2a2a2a,
      transparent: true,
      opacity: 0.8
    })
    const ground = new THREE.Mesh(groundGeometry, groundMaterial)
    ground.rotation.x = -Math.PI / 2
    ground.position.y = -5
    this.scene.add(ground)

    // 创建建筑物
    this.createBuildings()

    // 创建道路
    this.createRoads()
  }

  /**
   * 创建建筑物
   */
  private createBuildings(): void {
    const buildings = [
      { position: [0, 0, 0], size: [20, 30, 15], color: 0x4a90e2 },
      { position: [40, 0, 20], size: [15, 25, 12], color: 0x7ed321 },
      { position: [-30, 0, -20], size: [18, 20, 10], color: 0xf5a623 },
      { position: [60, 0, -40], size: [25, 35, 18], color: 0xbd10e0 }
    ]

    buildings.forEach(building => {
      const geometry = new THREE.BoxGeometry(...building.size)
      const material = new THREE.MeshLambertMaterial({ color: building.color })
      const mesh = new THREE.Mesh(geometry, material)
      mesh.position.set(...building.position)
      this.scene.add(mesh)
    })
  }

  /**
   * 创建道路
   */
  private createRoads(): void {
    const roadGeometry = new THREE.PlaneGeometry(200, 8)
    const roadMaterial = new THREE.MeshLambertMaterial({ color: 0x333333 })
    
    // 水平道路
    const horizontalRoad = new THREE.Mesh(roadGeometry, roadMaterial)
    horizontalRoad.rotation.x = -Math.PI / 2
    horizontalRoad.position.y = -4.9
    this.scene.add(horizontalRoad)

    // 垂直道路
    const verticalRoad = new THREE.Mesh(roadGeometry, roadMaterial)
    verticalRoad.rotation.x = -Math.PI / 2
    verticalRoad.rotation.z = Math.PI / 2
    verticalRoad.position.y = -4.9
    this.scene.add(verticalRoad)
  }

  /**
   * 添加摄像头
   */
  addCamera(cameraData: any): void {
    const { id, position, name } = cameraData

    // 创建摄像头3D模型
    const cameraGeometry = new THREE.CylinderGeometry(1, 1, 4, 8)
    const cameraMaterial = new THREE.MeshLambertMaterial({ color: 0x409eff })
    const cameraMesh = new THREE.Mesh(cameraGeometry, cameraMaterial)
    cameraMesh.position.set(position.x, position.y, position.z)
    cameraMesh.rotation.x = Math.PI / 2

    // 添加摄像头标签
    const canvas = document.createElement('canvas')
    const context = canvas.getContext('2d')!
    canvas.width = 256
    canvas.height = 64
    context.fillStyle = 'rgba(0, 0, 0, 0.8)'
    context.fillRect(0, 0, 256, 64)
    context.fillStyle = 'white'
    context.font = '16px Arial'
    context.textAlign = 'center'
    context.fillText(name, 128, 40)

    const texture = new THREE.CanvasTexture(canvas)
    const labelGeometry = new THREE.PlaneGeometry(8, 2)
    const labelMaterial = new THREE.MeshBasicMaterial({ 
      map: texture,
      transparent: true,
      side: THREE.DoubleSide
    })
    const label = new THREE.Mesh(labelGeometry, labelMaterial)
    label.position.set(position.x, position.y + 3, position.z)

    // 存储摄像头信息
    this.cameras.set(id, {
      mesh: cameraMesh,
      label: label,
      data: cameraData
    })

    this.scene.add(cameraMesh)
    this.scene.add(label)
  }

  /**
   * 高亮摄像头
   */
  highlightCamera(cameraId: string): void {
    this.cameras.forEach((camera, id) => {
      if (id === cameraId) {
        camera.mesh.material.color.setHex(0x67c23a)
        camera.mesh.scale.setScalar(1.2)
      } else {
        camera.mesh.material.color.setHex(0x409eff)
        camera.mesh.scale.setScalar(1.0)
      }
    })
  }

  /**
   * 显示警报
   */
  showAlert(cameraId: string, alertType: string): void {
    const camera = this.cameras.get(cameraId)
    if (!camera) return

    // 创建警报动画
    const alertGeometry = new THREE.SphereGeometry(2, 8, 6)
    const alertMaterial = new THREE.MeshBasicMaterial({ 
      color: 0xf56c6c,
      transparent: true,
      opacity: 0.8
    })
    const alertMesh = new THREE.Mesh(alertGeometry, alertMaterial)
    alertMesh.position.copy(camera.mesh.position)
    alertMesh.position.y += 5

    this.scene.add(alertMesh)

    // 警报动画
    const animate = () => {
      alertMesh.rotation.y += 0.1
      alertMesh.scale.x = 1 + Math.sin(Date.now() * 0.01) * 0.2
      alertMesh.scale.y = 1 + Math.sin(Date.now() * 0.01) * 0.2
      alertMesh.scale.z = 1 + Math.sin(Date.now() * 0.01) * 0.2
    }

    // 5秒后移除警报
    setTimeout(() => {
      this.scene.remove(alertMesh)
    }, 5000)

    // 添加到动画循环
    this.addAnimation(animate)
  }

  /**
   * 清除警报
   */
  clearAlert(cameraId: string): void {
    // 移除警报相关的动画
    this.removeAnimation()
  }

  /**
   * 视频纹理映射到建筑物
   */
  mapVideoToBuilding(cameraId: string, position: any): void {
    const camera = this.cameras.get(cameraId)
    if (!camera) return

    // 创建视频纹理
    const video = document.createElement('video')
    video.src = camera.data.streamUrl
    video.loop = true
    video.muted = true
    video.play()

    const videoTexture = new THREE.VideoTexture(video)
    videoTexture.minFilter = THREE.LinearFilter
    videoTexture.magFilter = THREE.LinearFilter

    // 创建建筑物表面材质
    const buildingMaterial = new THREE.MeshBasicMaterial({ 
      map: videoTexture,
      transparent: true,
      opacity: 0.8
    })

    // 应用到最近的建筑物
    this.scene.children.forEach(child => {
      if (child instanceof THREE.Mesh && child.geometry.type === 'BoxGeometry') {
        const distance = child.position.distanceTo(new THREE.Vector3(position.x, position.y, position.z))
        if (distance < 30) {
          child.material = buildingMaterial
        }
      }
    })
  }

  /**
   * 清除视频映射
   */
  clearVideoMapping(): void {
    // 恢复建筑物原始材质
    this.scene.children.forEach(child => {
      if (child instanceof THREE.Mesh && child.geometry.type === 'BoxGeometry') {
        child.material = new THREE.MeshLambertMaterial({ color: 0x4a90e2 })
      }
    })
  }

  /**
   * 显示追踪路径
   */
  showTrackingPath(trackingResults: any[]): void {
    // 创建追踪路径线条
    const points: THREE.Vector3[] = []
    
    trackingResults.forEach(result => {
      const camera = this.cameras.get(result.cameraId)
      if (camera) {
        points.push(camera.mesh.position.clone())
      }
    })

    if (points.length > 1) {
      const geometry = new THREE.BufferGeometry().setFromPoints(points)
      const material = new THREE.LineBasicMaterial({ color: 0x67c23a, linewidth: 3 })
      const line = new THREE.Line(geometry, material)
      this.scene.add(line)
    }
  }

  /**
   * 聚焦到摄像头
   */
  focusOnCamera(cameraId: string): void {
    const camera = this.cameras.get(cameraId)
    if (!camera) return

    // 移动相机到摄像头位置
    const targetPosition = camera.mesh.position.clone()
    targetPosition.y += 20
    targetPosition.z += 30

    // 相机动画
    const startPosition = this.camera.position.clone()
    const duration = 1000
    const startTime = Date.now()

    const animate = () => {
      const elapsed = Date.now() - startTime
      const progress = Math.min(elapsed / duration, 1)
      
      this.camera.position.lerpVectors(startPosition, targetPosition, progress)
      this.camera.lookAt(camera.mesh.position)

      if (progress < 1) {
        requestAnimationFrame(animate)
      }
    }

    animate()
  }

  /**
   * 开始渲染
   */
  startRender(): void {
    const animate = () => {
      this.animationId = requestAnimationFrame(animate)
      this.renderer.render(this.scene, this.camera)
    }
    animate()
  }

  /**
   * 添加动画
   */
  private addAnimation(animation: () => void): void {
    // 这里可以添加自定义动画
  }

  /**
   * 移除动画
   */
  private removeAnimation(): void {
    // 这里可以移除自定义动画
  }

  /**
   * 添加事件监听器
   */
  on(event: string, callback: Function): void {
    if (!this.eventListeners.has(event)) {
      this.eventListeners.set(event, [])
    }
    this.eventListeners.get(event)!.push(callback)
  }

  /**
   * 触发事件
   */
  private emit(event: string, data?: any): void {
    const listeners = this.eventListeners.get(event)
    if (listeners) {
      listeners.forEach(callback => callback(data))
    }
  }

  /**
   * 销毁引擎
   */
  destroy(): void {
    if (this.animationId) {
      cancelAnimationFrame(this.animationId)
    }
    
    if (this.renderer) {
      this.renderer.dispose()
    }
    
    if (this.container && this.renderer.domElement) {
      this.container.removeChild(this.renderer.domElement)
    }
  }

  /**
   * 处理窗口大小变化
   */
  onWindowResize(): void {
    this.camera.aspect = this.container.clientWidth / this.container.clientHeight
    this.camera.updateProjectionMatrix()
    this.renderer.setSize(this.container.clientWidth, this.container.clientHeight)
  }
} 