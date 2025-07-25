// 打包分析服务
interface BundleInfo {
  name: string
  size: number
  gzipSize: number
  modules: string[]
  dependencies: string[]
}

interface AnalysisResult {
  totalSize: number
  totalGzipSize: number
  bundles: BundleInfo[]
  duplicates: string[]
  unusedModules: string[]
  recommendations: string[]
}

class BundleAnalyzerService {
  private analysisData: AnalysisResult | null = null

  // 分析打包结果
  async analyzeBundles(): Promise<AnalysisResult> {
    try {
      // 获取打包信息（在生产环境中，这些数据通常来自webpack-bundle-analyzer）
      const bundleStats = await this.getBundleStats()
      
      const analysis: AnalysisResult = {
        totalSize: 0,
        totalGzipSize: 0,
        bundles: [],
        duplicates: [],
        unusedModules: [],
        recommendations: []
      }

      // 分析每个bundle
      for (const bundle of bundleStats) {
        const bundleInfo: BundleInfo = {
          name: bundle.name,
          size: bundle.size,
          gzipSize: bundle.gzipSize || bundle.size * 0.3, // 估算gzip大小
          modules: bundle.modules || [],
          dependencies: bundle.dependencies || []
        }

        analysis.bundles.push(bundleInfo)
        analysis.totalSize += bundleInfo.size
        analysis.totalGzipSize += bundleInfo.gzipSize
      }

      // 检测重复模块
      analysis.duplicates = this.findDuplicateModules(analysis.bundles)

      // 检测未使用的模块
      analysis.unusedModules = await this.findUnusedModules()

      // 生成优化建议
      analysis.recommendations = this.generateRecommendations(analysis)

      this.analysisData = analysis
      return analysis
    } catch (error) {
      console.error('Bundle analysis failed:', error)
      throw error
    }
  }

  // 获取打包统计信息
  private async getBundleStats(): Promise<any[]> {
    // 在实际项目中，这里会读取webpack或vite的打包统计文件
    // 这里提供模拟数据
    return [
      {
        name: 'vendor',
        size: 500000,
        gzipSize: 150000,
        modules: ['vue', 'vue-router', 'pinia', 'element-plus'],
        dependencies: ['vue', 'vue-router', 'pinia', 'element-plus']
      },
      {
        name: 'app',
        size: 200000,
        gzipSize: 60000,
        modules: ['src/main.ts', 'src/App.vue', 'src/components/*'],
        dependencies: ['vue']
      },
      {
        name: 'echarts',
        size: 800000,
        gzipSize: 240000,
        modules: ['echarts'],
        dependencies: ['echarts']
      }
    ]
  }

  // 查找重复模块
  private findDuplicateModules(bundles: BundleInfo[]): string[] {
    const moduleCount = new Map<string, number>()
    const duplicates: string[] = []

    bundles.forEach(bundle => {
      bundle.modules.forEach(module => {
        const count = moduleCount.get(module) || 0
        moduleCount.set(module, count + 1)
      })
    })

    moduleCount.forEach((count, module) => {
      if (count > 1) {
        duplicates.push(module)
      }
    })

    return duplicates
  }

  // 查找未使用的模块
  private async findUnusedModules(): Promise<string[]> {
    // 在实际项目中，这里会分析代码使用情况
    // 这里提供模拟数据
    return [
      'lodash/debounce', // 如果项目中有自己的防抖实现
      'moment', // 如果可以用更轻量的dayjs替代
      'axios/lib/adapters/http' // 浏览器环境不需要的模块
    ]
  }

  // 生成优化建议
  private generateRecommendations(analysis: AnalysisResult): string[] {
    const recommendations: string[] = []

    // 检查bundle大小
    analysis.bundles.forEach(bundle => {
      if (bundle.size > 1000000) { // 1MB
        recommendations.push(`${bundle.name} bundle过大 (${this.formatSize(bundle.size)})，建议进行代码分割`)
      }
    })

    // 检查重复模块
    if (analysis.duplicates.length > 0) {
      recommendations.push(`发现${analysis.duplicates.length}个重复模块，建议优化打包配置`)
    }

    // 检查未使用模块
    if (analysis.unusedModules.length > 0) {
      recommendations.push(`发现${analysis.unusedModules.length}个未使用模块，建议移除以减小打包体积`)
    }

    // 检查总体积
    if (analysis.totalSize > 2000000) { // 2MB
      recommendations.push('总打包体积过大，建议启用懒加载和代码分割')
    }

    // 检查gzip压缩率
    const compressionRatio = analysis.totalGzipSize / analysis.totalSize
    if (compressionRatio > 0.4) {
      recommendations.push('Gzip压缩率较低，建议检查是否包含大量二进制文件或已压缩文件')
    }

    return recommendations
  }

  // 格式化文件大小
  private formatSize(bytes: number): string {
    const sizes = ['B', 'KB', 'MB', 'GB']
    if (bytes === 0) return '0 B'
    const i = Math.floor(Math.log(bytes) / Math.log(1024))
    return `${(bytes / Math.pow(1024, i)).toFixed(2)} ${sizes[i]}`
  }

  // 获取分析结果
  getAnalysisResult(): AnalysisResult | null {
    return this.analysisData
  }

  // 导出分析报告
  exportReport(): string {
    if (!this.analysisData) {
      return 'No analysis data available'
    }

    const { totalSize, totalGzipSize, bundles, duplicates, unusedModules, recommendations } = this.analysisData

    let report = '# Bundle Analysis Report\n\n'
    
    report += `## Summary\n`
    report += `- Total Size: ${this.formatSize(totalSize)}\n`
    report += `- Total Gzipped Size: ${this.formatSize(totalGzipSize)}\n`
    report += `- Compression Ratio: ${((totalGzipSize / totalSize) * 100).toFixed(2)}%\n\n`

    report += `## Bundles\n`
    bundles.forEach(bundle => {
      report += `### ${bundle.name}\n`
      report += `- Size: ${this.formatSize(bundle.size)}\n`
      report += `- Gzipped: ${this.formatSize(bundle.gzipSize)}\n`
      report += `- Modules: ${bundle.modules.length}\n\n`
    })

    if (duplicates.length > 0) {
      report += `## Duplicate Modules\n`
      duplicates.forEach(module => {
        report += `- ${module}\n`
      })
      report += '\n'
    }

    if (unusedModules.length > 0) {
      report += `## Unused Modules\n`
      unusedModules.forEach(module => {
        report += `- ${module}\n`
      })
      report += '\n'
    }

    if (recommendations.length > 0) {
      report += `## Recommendations\n`
      recommendations.forEach(rec => {
        report += `- ${rec}\n`
      })
    }

    return report
  }
}

export const bundleAnalyzer = new BundleAnalyzerService()
export default bundleAnalyzer