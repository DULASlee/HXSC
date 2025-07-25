<template>
  <div class="digital-3d-view" ref="container">
    <div v-if="loading" class="loading-overlay">
      <el-icon class="loading-icon" :size="40"><Loading /></el-icon>
      <span>加载中...</span>
    </div>
    <div v-if="error" class="error-overlay">
      <el-icon class="error-icon" :size="40"><WarningFilled /></el-icon>
      <span>{{ error }}</span>
    </div>
    <canvas ref="canvas" class="canvas-3d"></canvas>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'
import { ElIcon } from 'element-plus'
import { Loading, WarningFilled } from '@element-plus/icons-vue'
import * as THREE from 'three'
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader.js'
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js'

const props = defineProps<{
  modelUrl: string
}>()

const container = ref<HTMLElement | null>(null)
const canvas = ref<HTMLCanvasElement | null>(null)
const loading = ref(true)
const error = ref<string | null>(null)

let renderer: THREE.WebGLRenderer | null = null
let scene: THREE.Scene | null = null
let camera: THREE.PerspectiveCamera | null = null
let controls: OrbitControls | null = null
let animationFrameId: number | null = null

onMounted(() => {
  if (canvas.value && container.value) {
    initThree(container.value, canvas.value)
    loadModel(props.modelUrl)
    animate()
  }
})

onUnmounted(() => {
  if (animationFrameId) {
    cancelAnimationFrame(animationFrameId)
  }
  if (renderer) {
    renderer.dispose()
  }
  if (controls) {
    controls.dispose()
  }
})

watch(
  () => props.modelUrl,
  (newUrl, oldUrl) => {
    if (newUrl && newUrl !== oldUrl) {
      loadModel(newUrl)
    }
  }
)

const initThree = (containerElement: HTMLElement, canvasElement: HTMLCanvasElement) => {
  // Scene
  scene = new THREE.Scene()
  scene.background = new THREE.Color(0x0a192f) // 深蓝色背景

  // Camera
  camera = new THREE.PerspectiveCamera(75, containerElement.clientWidth / containerElement.clientHeight, 0.1, 1000)
  camera.position.set(10, 10, 10)

  // Renderer
  renderer = new THREE.WebGLRenderer({ canvas: canvasElement, antialias: true })
  renderer.setSize(containerElement.clientWidth, containerElement.clientHeight)
  renderer.setPixelRatio(window.devicePixelRatio)

  // Controls
  controls = new OrbitControls(camera, renderer.domElement)
  controls.enableDamping = true
  controls.dampingFactor = 0.05

  // Lights
  const ambientLight = new THREE.AmbientLight(0xffffff, 0.5)
  scene.add(ambientLight)

  const directionalLight = new THREE.DirectionalLight(0xffffff, 1)
  directionalLight.position.set(5, 10, 7.5)
  scene.add(directionalLight)

  // Handle resize
  window.addEventListener('resize', onWindowResize)
}

const loadModel = (url: string) => {
  if (!scene) return

  loading.value = true
  error.value = null

  // Clear previous model
  const previousModel = scene.getObjectByName('gltf_model')
  if (previousModel) {
    scene.remove(previousModel)
  }

  const loader = new GLTFLoader()
  loader.load(
    url,
    gltf => {
      gltf.scene.name = 'gltf_model'
      scene!.add(gltf.scene)
      loading.value = false
    },
    undefined,
    err => {
      console.error('An error happened while loading the model:', err)
      error.value = '模型加载失败'
      loading.value = false
    }
  )
}

const animate = () => {
  animationFrameId = requestAnimationFrame(animate)

  if (controls) {
    controls.update()
  }

  if (renderer && scene && camera) {
    renderer.render(scene, camera)
  }
}

const onWindowResize = () => {
  if (camera && renderer && container.value) {
    camera.aspect = container.value.clientWidth / container.value.clientHeight
    camera.updateProjectionMatrix()
    renderer.setSize(container.value.clientWidth, container.value.clientHeight)
  }
}
</script>

<style scoped>
.digital-3d-view {
  width: 100%;
  height: 100%;
  position: relative;
  border-radius: 8px;
  overflow: hidden;
  background-color: #0a192f;
}

.canvas-3d {
  width: 100%;
  height: 100%;
  display: block;
}

.loading-overlay,
.error-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background-color: rgba(10, 25, 47, 0.8);
  color: #cdd6f4;
  z-index: 10;
}

.loading-icon,
.error-icon {
  margin-bottom: 16px;
}

.loading-icon {
  animation: spin 1.5s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}
</style>