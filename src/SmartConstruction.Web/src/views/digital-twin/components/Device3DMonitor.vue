<template>
  <div class="device-3d-monitor" ref="container">
    <div class="loading-overlay" v-if="loading">
      <el-icon class="loading-icon"><Loading /></el-icon>
      <span>加载3D模型中...</span>
    </div>
    <div class="device-controls" v-if="!loading">
      <div class="control-item">
        <el-tooltip content="旋转视角" placement="top">
          <el-button circle size="small" @click="toggleRotation">
            <el-icon><Refresh /></el-icon>
          </el-button>
        </el-tooltip>
      </div>
      <div class="control-item">
        <el-tooltip content="重置视角" placement="top">
          <el-button circle size="small" @click="resetCamera">
            <el-icon><View /></el-icon>
          </el-button>
        </el-tooltip>
      </div>
      <div class="control-item">
        <el-tooltip content="全屏查看" placement="top">
          <el-button circle size="small" @click="toggleFullscreen">
            <el-icon><FullScreen /></el-icon>
          </el-button>
        </el-tooltip>
      </div>
    </div>
    <div class="device-info" v-if="selectedDevice && !loading">
      <div class="info-header">
        <h4>{{ selectedDevice.name }}</h4>
        <el-tag :type="getStatusType(selectedDevice.status)" size="small">
          {{ getStatusText(selectedDevice.status) }}
        </el-tag>
      </div>
      <div class="info-content">
        <div class="info-item">
          <span class="label">当前负载:</span>
          <span class="value">{{ selectedDevice.realTimeData.load }}kg / {{ selectedDevice.realTimeData.maxLoad }}kg</span>
          <el-progress 
            :percentage="Math.round((selectedDevice.realTimeData.load / selectedDevice.realTimeData.maxLoad) * 100)" 
            :stroke-width="4" 
            :color="getLoadColor(selectedDevice.realTimeData.load, selectedDevice.realTimeData.maxLoad)"
          />
        </div>
        <div class="info-item">
          <span class="label">当前高度:</span>
          <span class="value">{{ selectedDevice.realTimeData.height }}m / {{ selectedDevice.realTimeData.maxHeight }}m</span>
          <el-progress 
            :percentage="Math.round((selectedDevice.realTimeData.height / selectedDevice.realTimeData.maxHeight) * 100)" 
            :stroke-width="4" 
            :color="getHeightColor(selectedDevice.realTimeData.height, selectedDevice.realTimeData.maxHeight)"
          />
        </div>
        <div class="info-item">
          <span class="label">风速:</span>
          <span class="value">{{ selectedDevice.realTimeData.windSpeed }}m/s</span>
          <el-progress 
            :percentage="Math.round((selectedDevice.realTimeData.windSpeed / selectedDevice.realTimeData.maxWindSpeed) * 100)" 
            :stroke-width="4" 
            :color="getWindColor(selectedDevice.realTimeData.windSpeed, selectedDevice.realTimeData.maxWindSpeed)"
          />
        </div>
        <div class="info-item" v-if="selectedDevice.type === 'TowerCrane'">
          <span class="label">旋转角度:</span>
          <span class="value">{{ selectedDevice.realTimeData.rotation }}°</span>
        </div>
        <div class="info-item">
          <span class="label">工作时长:</span>
          <span class="value">{{ selectedDevice.realTimeData.workingHours }}小时</span>
        </div>
      </div>
      <div class="safety-status">
        <div class="safety-header">
          <span>安全状态</span>
          <el-tag :type="getSafetyScoreType(selectedDevice.safetyStatus.safetyScore)" size="small">
            {{ selectedDevice.safetyStatus.safetyScore }}分
          </el-tag>
        </div>
        <div class="safety-alerts">
          <div class="alert-item" :class="{ 'alert-active': selectedDevice.safetyStatus.overload }">
            <el-icon><Warning /></el-icon>
            <span>超载</span>
          </div>
          <div class="alert-item" :class="{ 'alert-active': selectedDevice.safetyStatus.overHeight }">
            <el-icon><Warning /></el-icon>
            <span>超高</span>
          </div>
          <div class="alert-item" :class="{ 'alert-active': selectedDevice.safetyStatus.highWind }">
            <el-icon><Warning /></el-icon>
            <span>大风</span>
          </div>
          <div class="alert-item" :class="{ 'alert-active': selectedDevice.safetyStatus.collision }">
            <el-icon><Warning /></el-icon>
            <span>碰撞</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'
import { Loading, Refresh, View, FullScreen, Warning } from '@element-plus/icons-vue'
import * as THREE from 'three'
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls'
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader'

const props = defineProps({
  device: {
    type: Object,
    required: true
  },
  autoRotate: {
    type: Boolean,
    default: true
  }
})

const emit = defineEmits(['device-selected'])

// 状态变量
const container = ref<HTMLElement | null>(null)
const loading = ref(true)
const selectedDevice = ref(props.device)
const isRotating = ref(props.autoRotate)

// Three.js 变量
let scene: THREE.Scene
let camera: THREE.PerspectiveCamera
let renderer: THREE.WebGLRenderer
let controls: OrbitControls
let model: THREE.Group
let animationFrameId: number

// 初始化3D场景
const initScene = () => {
  if (!container.value) return
  
  // 创建场景
  scene = new THREE.Scene()
  scene.background = new THREE.Color(0x1a1a2e)
  
  // 创建相机
  const width = container.value.clientWidth
  const height = container.value.clientHeight
  camera = new THREE.PerspectiveCamera(45, width / height, 0.1, 1000)
  camera.position.set(5, 5, 10)
  
  // 创建渲染器
  renderer = new THREE.WebGLRenderer({ antialias: true })
  renderer.setSize(width, height)
  renderer.setPixelRatio(window.devicePixelRatio)
  renderer.shadowMap.enabled = true
  container.value.appendChild(renderer.domElement)
  
  // 添加控制器
  controls = new OrbitControls(camera, renderer.domElement)
  controls.enableDamping = true
  controls.dampingFactor = 0.05
  
  // 添加灯光
  const ambientLight = new THREE.AmbientLight(0xffffff, 0.5)
  scene.add(ambientLight)
  
  const directionalLight = new THREE.DirectionalLight(0xffffff, 1)
  directionalLight.position.set(5, 10, 7.5)
  directionalLight.castShadow = true
  scene.add(directionalLight)
  
  // 添加地面
  const groundGeometry = new THREE.PlaneGeometry(20, 20)
  const groundMaterial = new THREE.MeshStandardMaterial({ 
    color: 0x333333,
    roughness: 0.8,
    metalness: 0.2
  })
  const ground = new THREE.Mesh(groundGeometry, groundMaterial)
  ground.rotation.x = -Math.PI / 2
  ground.receiveShadow = true
  scene.add(ground)
  
  // 加载3D模型
  loadModel()
  
  // 开始动画循环
  animate()
  
  // 添加窗口大小变化监听
  window.addEventListener('resize', onWindowResize)
}

// 加载3D模型
const loadModel = () => {
  loading.value = true
  
  const loader = new GLTFLoader()
  
  // 根据设备类型加载不同模型
  const modelPath = selectedDevice.value.type === 'TowerCrane' 
    ? '/models/tower_crane.glb' 
    : '/models/elevator.glb'
  
  loader.load(
    modelPath,
    (gltf) => {
      if (model) {
        scene.remove(model)
      }
      model = gltf.scene
      model.traverse(child => {
        if (child instanceof THREE.Mesh) {
          child.castShadow = true
          child.receiveShadow = true
        }
      })
      scene.add(model)
      loading.value = false
    },
    (xhr) => {
      // 可以选择性地显示加载进度
      // console.log((xhr.loaded / xhr.total * 100) + '% loaded')
    },
    (error) => {
      console.error('An error happened while loading the model', error)
      loading.value = false
      // 可以在这里显示一个错误提示
    }
  )
}

// 动画循环
const animate = () => {
  animationFrameId = requestAnimationFrame(animate)
  
  if (isRotating.value && model) {
    model.rotation.y += 0.005
  }
  
  if (controls) {
    controls.update()
  }
  
  renderer.render(scene, camera)
}

// 窗口大小变化处理
const onWindowResize = () => {
  if (!container.value) return
  
  const width = container.value.clientWidth
  const height = container.value.clientHeight
  
  camera.aspect = width / height
  camera.updateProjectionMatrix()
  
  renderer.setSize(width, height)
}

// 切换旋转
const toggleRotation = () => {
  isRotating.value = !isRotating.value
}

// 重置相机
const resetCamera = () => {
  if (!controls) return
  
  controls.reset()
}

// 切换全屏
const toggleFullscreen = () => {
  if (!container.value) return
  
  if (!document.fullscreenElement) {
    container.value.requestFullscreen().catch(err => {
      console.error(`Error attempting to enable full-screen mode: ${err.message}`)
    })
  } else {
    document.exitFullscreen()
  }
}

// 工具函数
const getStatusType = (status: string) => {
  const statusMap: Record<string, string> = {
    'Running': 'success',
    'Idle': 'info',
    'Maintenance': 'warning',
    'Fault': 'danger'
  }
  return statusMap[status] || 'info'
}

const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'Running': '运行中',
    'Idle': '空闲',
    'Maintenance': '维护中',
    'Fault': '故障'
  }
  return statusMap[status] || status
}

const getLoadColor = (load: number, maxLoad: number) => {
  const percentage = (load / maxLoad) * 100
  if (percentage > 90) return '#e74c3c'
  if (percentage > 75) return '#f39c12'
  return '#2ecc71'
}

const getHeightColor = (height: number, maxHeight: number) => {
  const percentage = (height / maxHeight) * 100
  if (percentage > 90) return '#f39c12'
  return '#3498db'
}

const getWindColor = (windSpeed: number, maxWindSpeed: number) => {
  const percentage = (windSpeed / maxWindSpeed) * 100
  if (percentage > 80) return '#e74c3c'
  if (percentage > 60) return '#f39c12'
  return '#2ecc71'
}

const getSafetyScoreType = (score: number) => {
  if (score >= 90) return 'success'
  if (score >= 75) return 'warning'
  return 'danger'
}

// 监听设备变化
watch(() => props.device, (newDevice) => {
  if (newDevice) {
    selectedDevice.value = newDevice
    loadModel()
  }
}, { deep: true, immediate: true })

// 生命周期钩子
onMounted(() => {
  initScene()
})

onUnmounted(() => {
  // 清理资源
  if (animationFrameId) {
    cancelAnimationFrame(animationFrameId)
  }
  
  if (renderer) {
    renderer.dispose()
  }
  
  if (container.value && renderer) {
    container.value.removeChild(renderer.domElement)
  }
  
  window.removeEventListener('resize', onWindowResize)
})
</script>

<style lang="scss" scoped>
.device-3d-monitor {
  position: relative;
  width: 100%;
  height: 400px;
  border-radius: 8px;
  overflow: hidden;
  background-color: #1a1a2e;
  
  .loading-overlay {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    background-color: rgba(26, 26, 46, 0.8);
    z-index: 10;
    
    .loading-icon {
      font-size: 32px;
      color: #3498db;
      animation: spin 1.5s linear infinite;
    }
    
    span {
      margin-top: 12px;
      color: #ffffff;
      font-size: 14px;
    }
  }
  
  .device-controls {
    position: absolute;
    top: 16px;
    right: 16px;
    display: flex;
    flex-direction: column;
    gap: 8px;
    z-index: 5;
    
    .control-item {
      .el-button {
        background-color: rgba(52, 152, 219, 0.2);
        border-color: rgba(52, 152, 219, 0.5);
        color: #ffffff;
        
        &:hover {
          background-color: rgba(52, 152, 219, 0.4);
        }
      }
    }
  }
  
  .device-info {
    position: absolute;
    bottom: 16px;
    left: 16px;
    width: 300px;
    background-color: rgba(26, 26, 46, 0.8);
    border: 1px solid rgba(52, 152, 219, 0.5);
    border-radius: 8px;
    padding: 12px;
    z-index: 5;
    
    .info-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 12px;
      
      h4 {
        margin: 0;
        color: #ffffff;
        font-size: 16px;
      }
    }
    
    .info-content {
      .info-item {
        margin-bottom: 8px;
        
        .label {
          font-size: 12px;
          color: #7f8c8d;
          margin-right: 8px;
        }
        
        .value {
          font-size: 14px;
          color: #ffffff;
          font-weight: 500;
        }
      }
    }
    
    .safety-status {
      margin-top: 16px;
      padding-top: 12px;
      border-top: 1px solid rgba(255, 255, 255, 0.1);
      
      .safety-header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 12px;
        
        span {
          color: #ffffff;
          font-size: 14px;
        }
      }
      
      .safety-alerts {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 8px;
        
        .alert-item {
          display: flex;
          align-items: center;
          gap: 8px;
          padding: 6px 12px;
          border-radius: 4px;
          background-color: rgba(255, 255, 255, 0.05);
          color: #7f8c8d;
          
          &.alert-active {
            background-color: rgba(231, 76, 60, 0.2);
            color: #e74c3c;
          }
        }
      }
    }
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>