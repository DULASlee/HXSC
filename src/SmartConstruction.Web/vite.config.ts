import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import vueJsx from "@vitejs/plugin-vue-jsx";
import AutoImport from "unplugin-auto-import/vite";
import Components from "unplugin-vue-components/vite";
import { ElementPlusResolver } from "unplugin-vue-components/resolvers";
import viteCompression from "vite-plugin-compression";
import { resolve } from "path";
import fs from 'fs';
import path from 'path';

// 创建一个插件来处理动态导入
function dynamicImportPlugin() {
  const viewsDir = resolve(__dirname, 'src/views');
  const moduleCache = new Map();

  // 递归扫描目录获取所有 Vue 文件
  function scanVueFiles(dir, baseDir = '') {
    const result = [];
    const files = fs.readdirSync(dir);
    
    for (const file of files) {
      const filePath = path.join(dir, file);
      const relativePath = path.join(baseDir, file);
      const stat = fs.statSync(filePath);
      
      if (stat.isDirectory()) {
        result.push(...scanVueFiles(filePath, relativePath));
      } else if (file.endsWith('.vue')) {
        // 去掉 .vue 后缀
        const name = relativePath.replace(/\.vue$/, '');
        result.push(name);
      }
    }
    
    return result;
  }

  return {
    name: 'vite-plugin-dynamic-import',
    configureServer(server) {
      // 在开发服务器启动时扫描一次
      const vueFiles = scanVueFiles(viewsDir);
      vueFiles.forEach(file => {
        moduleCache.set(file.replace(/\\/g, '/'), true);
      });
      
      console.log('已扫描 Vue 组件文件:', moduleCache.size);
    },
    resolveId(id) {
      // 处理动态导入的虚拟模块
      if (id.startsWith('virtual:vue-component/')) {
        return id;
      }
      return null;
    },
    load(id) {
      // 加载虚拟模块
      if (id.startsWith('virtual:vue-component/')) {
        const componentPath = id.replace('virtual:vue-component/', '');
        return `export { default } from '@/views/${componentPath}.vue'`;
      }
      return null;
    }
  };
}

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueJsx(),
    AutoImport({
      resolvers: [ElementPlusResolver()],
      imports: ["vue", "vue-router", "pinia"],
      // 使用绝对路径确保插件能正确扫描
      dirs: [
        resolve(__dirname, 'src/stores'),
        resolve(__dirname, 'src/composables'),
        resolve(__dirname, 'src/api/modules')
      ],
      dts: "src/auto-imports.d.ts",
    }),
    Components({
      resolvers: [ElementPlusResolver()],
      // 开启深度扫描，并指定扫描目录
      dirs: ['src/components'],
      deep: true,
      dts: 'src/components.d.ts'
    }),
    viteCompression({
      verbose: true,
      disable: false,
      threshold: 10240,
      algorithm: "gzip",
      ext: ".gz",
    }),
    dynamicImportPlugin(),
  ],
  resolve: {
    alias: {
      "@": resolve(__dirname, "src"),
    },
  },
  optimizeDeps: {
    include: [
      'three',
      'three/examples/jsm/controls/OrbitControls',
      'three/examples/jsm/loaders/GLTFLoader'
    ]
  },
  css: {
    preprocessorOptions: {
      scss: {
        additionalData: `
          @use "@/assets/styles/variables.scss" as *;
          @use "@/assets/styles/mixins.scss" as *;
        `,
        api: 'modern-compiler'
      },
    },
  },
  server: {
    host: true,
    port: 3000,
    open: true,
    cors: true,
    proxy: {
      '/api': {
        target: 'http://localhost:8998',
        changeOrigin: true,
        // 如果后端API没有/api前缀，可以取消下面的注释
        // rewrite: (path) => path.replace(/^\/api/, '')
      }
    }
  },
  build: {
    minify: "terser",
    terserOptions: {
      compress: {
        drop_console: true,
        drop_debugger: true,
      },
    },
    rollupOptions: {
      external: [
        'three',
        'three/examples/jsm/controls/OrbitControls',
        'three/examples/jsm/loaders/GLTFLoader'
      ],
      output: {
        chunkFileNames: "js/[name]-[hash].js",
        entryFileNames: "js/[name]-[hash].js",
        assetFileNames: "[ext]/[name]-[hash].[ext]",
      },
    },
  },
});
