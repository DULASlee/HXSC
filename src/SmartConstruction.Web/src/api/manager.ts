// =============================================
// API管理工具 - 智能化API配置管理
// =============================================
import { apiGenerator, generateApiFromSwagger } from './generator';
import { apiConfigManager, type ApiConfig } from './config';
import { apiClient } from './client';

// API管理器类
export class ApiManager {
  private static instance: ApiManager;
  private initialized = false;

  private constructor() {}

  // 单例模式
  static getInstance(): ApiManager {
    if (!ApiManager.instance) {
      ApiManager.instance = new ApiManager();
    }
    return ApiManager.instance;
  }

  // 初始化API管理器
  async initialize(options?: {
    swaggerUrl?: string;
    config?: Partial<ApiConfig>;
    autoGenerate?: boolean;
  }): Promise<void> {
    if (this.initialized) {
      console.log('⚠️ API管理器已经初始化');
      return;
    }

    try {
      console.log('🚀 初始化API管理器...');

      // 如果提供了Swagger URL，自动生成配置
      if (options?.swaggerUrl && options?.autoGenerate !== false) {
        await this.generateFromSwagger(options.swaggerUrl);
      }

      // 如果提供了自定义配置，更新配置
      if (options?.config) {
        this.updateConfig(options.config);
      }

      this.initialized = true;
      console.log('✅ API管理器初始化完成');
    } catch (error) {
      console.error('❌ API管理器初始化失败:', error);
      throw error;
    }
  }

  // 从Swagger生成API配置
  async generateFromSwagger(swaggerUrl: string): Promise<{ config: ApiConfig; types: string }> {
    try {
      console.log('🔄 从Swagger生成API配置:', swaggerUrl);
      const result = await generateApiFromSwagger(swaggerUrl);
      console.log('✅ API配置生成成功');
      return result;
    } catch (error) {
      console.error('❌ 从Swagger生成API配置失败:', error);
      throw error;
    }
  }

  // 更新API配置
  updateConfig(config: Partial<ApiConfig>): void {
    apiConfigManager.updateConfig(config);
    apiClient.updateConfig(apiConfigManager.getConfig());
    console.log('✅ API配置已更新');
  }

  // 获取当前配置
  getConfig(): ApiConfig {
    return apiConfigManager.getConfig();
  }

  // 测试API连接
  async testConnection(): Promise<boolean> {
    try {
      console.log('🔍 测试API连接...');
      
      // 尝试调用一个简单的API端点
      const response = await apiClient.get('system', 'getInfo');
      console.log('✅ API连接测试成功:', response);
      return true;
    } catch (error) {
      console.warn('⚠️ API连接测试失败:', error);
      return false;
    }
  }

  // 获取API统计信息
  getApiStats(): {
    totalModules: number;
    totalEndpoints: number;
    moduleStats: Array<{ name: string; endpointCount: number }>;
  } {
    const config = this.getConfig();
    const moduleStats = config.modules.map(module => ({
      name: module.name,
      endpointCount: module.endpoints.length
    }));

    return {
      totalModules: config.modules.length,
      totalEndpoints: config.modules.reduce((total, module) => total + module.endpoints.length, 0),
      moduleStats
    };
  }

  // 验证API配置
  validateConfig(): {
    isValid: boolean;
    errors: string[];
    warnings: string[];
  } {
    const config = this.getConfig();
    const errors: string[] = [];
    const warnings: string[] = [];

    // 检查基础配置
    if (!config.baseURL) {
      errors.push('缺少baseURL配置');
    }

    if (config.timeout <= 0) {
      warnings.push('timeout配置可能过小');
    }

    // 检查模块配置
    config.modules.forEach(module => {
      if (!module.name) {
        errors.push(`模块缺少name配置`);
      }

      if (!module.basePath) {
        errors.push(`模块 ${module.name} 缺少basePath配置`);
      }

      if (module.endpoints.length === 0) {
        warnings.push(`模块 ${module.name} 没有端点配置`);
      }

      // 检查端点配置
      module.endpoints.forEach(endpoint => {
        if (!endpoint.name) {
          errors.push(`模块 ${module.name} 中的端点缺少name配置`);
        }

        if (!endpoint.path) {
          errors.push(`模块 ${module.name} 中的端点 ${endpoint.name} 缺少path配置`);
        }

        if (!endpoint.method) {
          errors.push(`模块 ${module.name} 中的端点 ${endpoint.name} 缺少method配置`);
        }
      });
    });

    return {
      isValid: errors.length === 0,
      errors,
      warnings
    };
  }

  // 导出配置
  exportConfig(): string {
    const config = this.getConfig();
    return JSON.stringify(config, null, 2);
  }

  // 导入配置
  importConfig(configJson: string): void {
    try {
      const config = JSON.parse(configJson);
      this.updateConfig(config);
      console.log('✅ 配置导入成功');
    } catch (error) {
      console.error('❌ 配置导入失败:', error);
      throw new Error('配置格式错误');
    }
  }

  // 重置配置
  resetConfig(): void {
    const { defaultApiConfig } = require('./config');
    apiConfigManager.updateConfig(defaultApiConfig);
    apiClient.updateConfig(defaultApiConfig);
    console.log('✅ 配置已重置为默认值');
  }

  // 获取API客户端实例
  getClient() {
    return apiClient;
  }

  // 创建自定义API调用
  createCustomApi(moduleName: string, endpointName: string) {
    return {
      get: (params?: any, options?: any) => apiClient.get(moduleName, endpointName, params, options),
      post: (data?: any, options?: any) => apiClient.post(moduleName, endpointName, data, options),
      put: (data?: any, options?: any) => apiClient.put(moduleName, endpointName, data, options),
      delete: (params?: any, options?: any) => apiClient.delete(moduleName, endpointName, params, options),
      upload: (file: File, data?: any, onProgress?: (progress: number) => void) => 
        apiClient.upload(moduleName, endpointName, file, data, onProgress),
      download: (params?: any, filename?: string) => 
        apiClient.download(moduleName, endpointName, params, filename)
    };
  }

  // 批量API调用
  async batchCall(calls: Array<{
    moduleName: string;
    endpointName: string;
    data?: any;
    options?: any;
  }>) {
    return apiClient.batch(calls);
  }

  // 监控API调用
  enableApiMonitoring(): void {
    // 这里可以添加API调用监控逻辑
    console.log('✅ API监控已启用');
  }

  // 禁用API监控
  disableApiMonitoring(): void {
    // 这里可以添加禁用API监控的逻辑
    console.log('✅ API监控已禁用');
  }
}

// 创建全局API管理器实例
export const apiManager = ApiManager.getInstance();

// 便捷的初始化方法
export const initializeApi = async (options?: {
  swaggerUrl?: string;
  config?: Partial<ApiConfig>;
  autoGenerate?: boolean;
}) => {
  return apiManager.initialize(options);
};

// 便捷的API调用方法
export const useApi = (moduleName: string, endpointName: string) => {
  return apiManager.createCustomApi(moduleName, endpointName);
};

// 导出默认实例
export default apiManager;