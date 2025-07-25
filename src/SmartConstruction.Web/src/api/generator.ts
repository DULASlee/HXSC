// =============================================
// OpenAPI类型生成器和配置管理
// =============================================
import { apiConfigManager, type ApiConfig, type ApiModule, type ApiEndpoint } from './config';

// OpenAPI规范接口
export interface OpenAPISpec {
  openapi: string;
  info: {
    title: string;
    version: string;
    description?: string;
  };
  servers: Array<{
    url: string;
    description?: string;
  }>;
  paths: Record<string, Record<string, any>>;
  components?: {
    schemas?: Record<string, any>;
    securitySchemes?: Record<string, any>;
  };
}

// API生成器类
export class ApiGenerator {
  private spec: OpenAPISpec | null = null;

  // 从URL加载OpenAPI规范
  async loadFromUrl(url: string): Promise<void> {
    try {
      const response = await fetch(url);
      if (!response.ok) {
        throw new Error(`Failed to fetch OpenAPI spec: ${response.statusText}`);
      }
      this.spec = await response.json();
      console.log('✅ OpenAPI规范加载成功:', this.spec.info);
    } catch (error) {
      console.error('❌ 加载OpenAPI规范失败:', error);
      throw error;
    }
  }

  // 从JSON对象加载OpenAPI规范
  loadFromObject(spec: OpenAPISpec): void {
    this.spec = spec;
    console.log('✅ OpenAPI规范加载成功:', this.spec.info);
  }

  // 生成API配置
  generateApiConfig(): ApiConfig {
    if (!this.spec) {
      throw new Error('OpenAPI规范未加载');
    }

    const baseURL = this.spec.servers?.[0]?.url || '';
    const modules: ApiModule[] = [];

    // 按路径前缀分组生成模块
    const pathGroups = this.groupPathsByPrefix(this.spec.paths);

    Object.entries(pathGroups).forEach(([prefix, paths]) => {
      const moduleName = this.extractModuleName(prefix);
      const endpoints: ApiEndpoint[] = [];

      Object.entries(paths).forEach(([path, methods]) => {
        Object.entries(methods).forEach(([method, operation]) => {
          const endpoint = this.createEndpoint(path, method, operation);
          if (endpoint) {
            endpoints.push(endpoint);
          }
        });
      });

      if (endpoints.length > 0) {
        modules.push({
          name: moduleName,
          basePath: prefix,
          endpoints
        });
      }
    });

    return {
      baseURL,
      timeout: 30000,
      modules
    };
  }

  // 按路径前缀分组
  private groupPathsByPrefix(paths: Record<string, any>): Record<string, Record<string, any>> {
    const groups: Record<string, Record<string, any>> = {};

    Object.entries(paths).forEach(([path, methods]) => {
      const prefix = this.extractPathPrefix(path);
      if (!groups[prefix]) {
        groups[prefix] = {};
      }
      groups[prefix][path] = methods;
    });

    return groups;
  }

  // 提取路径前缀
  private extractPathPrefix(path: string): string {
    const segments = path.split('/').filter(Boolean);
    if (segments.length >= 2) {
      return `/${segments[0]}/${segments[1]}`;
    }
    return `/${segments[0] || ''}`;
  }

  // 提取模块名称
  private extractModuleName(prefix: string): string {
    const segments = prefix.split('/').filter(Boolean);
    return segments[segments.length - 1] || 'default';
  }

  // 创建端点配置
  private createEndpoint(path: string, method: string, operation: any): ApiEndpoint | null {
    if (!operation || typeof operation !== 'object') {
      return null;
    }

    const endpointName = operation.operationId || this.generateEndpointName(path, method);
    const relativePath = this.getRelativePath(path);

    return {
      name: endpointName,
      path: relativePath,
      method: method.toUpperCase() as any,
      description: operation.summary || operation.description,
      params: this.extractParameters(operation.parameters),
      body: this.extractRequestBody(operation.requestBody),
      response: this.extractResponse(operation.responses)
    };
  }

  // 生成端点名称
  private generateEndpointName(path: string, method: string): string {
    const segments = path.split('/').filter(Boolean);
    const lastSegment = segments[segments.length - 1];
    
    // 移除路径参数标记
    const cleanSegment = lastSegment?.replace(/[{}]/g, '') || 'default';
    
    const methodMap: Record<string, string> = {
      'get': segments.length > 2 && lastSegment?.includes('{') ? 'getById' : 'getList',
      'post': 'create',
      'put': 'update',
      'patch': 'update',
      'delete': 'delete'
    };

    return methodMap[method.toLowerCase()] || `${method.toLowerCase()}${this.capitalize(cleanSegment)}`;
  }

  // 获取相对路径
  private getRelativePath(fullPath: string): string {
    const segments = fullPath.split('/').filter(Boolean);
    if (segments.length > 2) {
      return '/' + segments.slice(2).join('/');
    }
    return '';
  }

  // 提取参数
  private extractParameters(parameters?: any[]): Record<string, any> | undefined {
    if (!parameters || !Array.isArray(parameters)) {
      return undefined;
    }

    const params: Record<string, any> = {};
    parameters.forEach(param => {
      if (param.name && param.schema) {
        params[param.name] = this.getTypeFromSchema(param.schema);
      }
    });

    return Object.keys(params).length > 0 ? params : undefined;
  }

  // 提取请求体
  private extractRequestBody(requestBody?: any): Record<string, any> | undefined {
    if (!requestBody || !requestBody.content) {
      return undefined;
    }

    const jsonContent = requestBody.content['application/json'];
    if (jsonContent && jsonContent.schema) {
      return this.extractSchemaProperties(jsonContent.schema);
    }

    return undefined;
  }

  // 提取响应
  private extractResponse(responses?: any): Record<string, any> | undefined {
    if (!responses) {
      return undefined;
    }

    const successResponse = responses['200'] || responses['201'];
    if (successResponse && successResponse.content) {
      const jsonContent = successResponse.content['application/json'];
      if (jsonContent && jsonContent.schema) {
        return this.extractSchemaProperties(jsonContent.schema);
      }
    }

    return undefined;
  }

  // 提取Schema属性
  private extractSchemaProperties(schema: any): Record<string, any> {
    if (schema.properties) {
      const props: Record<string, any> = {};
      Object.entries(schema.properties).forEach(([key, value]: [string, any]) => {
        props[key] = this.getTypeFromSchema(value);
      });
      return props;
    }

    return { type: this.getTypeFromSchema(schema) };
  }

  // 从Schema获取类型
  private getTypeFromSchema(schema: any): string {
    if (!schema) return 'any';

    if (schema.type) {
      switch (schema.type) {
        case 'string':
          return schema.format === 'date-time' ? 'Date' : 'string';
        case 'number':
        case 'integer':
          return 'number';
        case 'boolean':
          return 'boolean';
        case 'array':
          const itemType = this.getTypeFromSchema(schema.items);
          return `${itemType}[]`;
        case 'object':
          return 'Record<string, any>';
        default:
          return 'any';
      }
    }

    if (schema.$ref) {
      const refName = schema.$ref.split('/').pop();
      return refName || 'any';
    }

    return 'any';
  }

  // 首字母大写
  private capitalize(str: string): string {
    return str.charAt(0).toUpperCase() + str.slice(1);
  }

  // 生成TypeScript类型定义
  generateTypeDefinitions(): string {
    if (!this.spec) {
      throw new Error('OpenAPI规范未加载');
    }

    let types = '// =============================================\n';
    types += '// 自动生成的API类型定义\n';
    types += '// =============================================\n\n';

    // 生成基础响应类型
    types += 'export interface ApiResponse<T = any> {\n';
    types += '  success: boolean;\n';
    types += '  message: string;\n';
    types += '  data: T;\n';
    types += '  code: number;\n';
    types += '  timestamp?: number;\n';
    types += '}\n\n';

    // 生成分页响应类型
    types += 'export interface PagedResult<T> {\n';
    types += '  items: T[];\n';
    types += '  total: number;\n';
    types += '  page: number;\n';
    types += '  pageSize: number;\n';
    types += '  totalPages: number;\n';
    types += '}\n\n';

    // 生成Schema类型
    if (this.spec.components?.schemas) {
      Object.entries(this.spec.components.schemas).forEach(([name, schema]) => {
        types += this.generateInterfaceFromSchema(name, schema);
      });
    }

    return types;
  }

  // 从Schema生成接口
  private generateInterfaceFromSchema(name: string, schema: any): string {
    let interfaceStr = `export interface ${name} {\n`;

    if (schema.properties) {
      Object.entries(schema.properties).forEach(([propName, propSchema]: [string, any]) => {
        const isRequired = schema.required?.includes(propName);
        const propType = this.getTypeFromSchema(propSchema);
        const optional = isRequired ? '' : '?';
        
        if (propSchema.description) {
          interfaceStr += `  /** ${propSchema.description} */\n`;
        }
        
        interfaceStr += `  ${propName}${optional}: ${propType};\n`;
      });
    }

    interfaceStr += '}\n\n';
    return interfaceStr;
  }

  // 保存配置到文件
  async saveConfigToFile(config: ApiConfig, filePath: string = 'src/api/generated-config.json'): Promise<void> {
    try {
      const configJson = JSON.stringify(config, null, 2);
      // 这里应该使用文件系统API保存文件
      console.log('生成的API配置:', configJson);
      
      // 在浏览器环境中，我们可以触发下载
      if (typeof window !== 'undefined') {
        const blob = new Blob([configJson], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'api-config.json';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
      }
    } catch (error) {
      console.error('保存配置文件失败:', error);
      throw error;
    }
  }

  // 保存类型定义到文件
  async saveTypesToFile(types: string, filePath: string = 'src/api/generated-types.d.ts'): Promise<void> {
    try {
      // 在浏览器环境中，我们可以触发下载
      if (typeof window !== 'undefined') {
        const blob = new Blob([types], { type: 'text/typescript' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'api-types.d.ts';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
      }
    } catch (error) {
      console.error('保存类型文件失败:', error);
      throw error;
    }
  }

  // 更新API配置管理器
  updateApiConfigManager(config: ApiConfig): void {
    apiConfigManager.updateConfig(config);
    console.log('✅ API配置管理器已更新');
  }
}

// 创建生成器实例
export const apiGenerator = new ApiGenerator();

// 便捷方法
export const generateApiFromSwagger = async (swaggerUrl: string) => {
  try {
    console.log('🚀 开始从Swagger生成API配置...');
    
    // 加载OpenAPI规范
    await apiGenerator.loadFromUrl(swaggerUrl);
    
    // 生成API配置
    const config = apiGenerator.generateApiConfig();
    console.log('✅ API配置生成成功:', config);
    
    // 生成类型定义
    const types = apiGenerator.generateTypeDefinitions();
    console.log('✅ 类型定义生成成功');
    
    // 更新配置管理器
    apiGenerator.updateApiConfigManager(config);
    
    // 保存文件
    await apiGenerator.saveConfigToFile(config);
    await apiGenerator.saveTypesToFile(types);
    
    console.log('🎉 API生成完成！');
    return { config, types };
  } catch (error) {
    console.error('❌ API生成失败:', error);
    throw error;
  }
};

export default apiGenerator;