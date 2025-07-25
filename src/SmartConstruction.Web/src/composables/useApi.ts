// =============================================
// API组合式函数 - 智能化、自动化API调用
// =============================================
import { ref, reactive, computed, onUnmounted, toValue, type MaybeRef } from 'vue';
import { ElMessage } from 'element-plus';

// API状态接口
export interface ApiState<T = any> {
  data: T | null;
  loading: boolean;
  error: any | null; // 存储完整的错误对象
  success: boolean;
}

// API选项接口
export interface UseApiOptions<T = any> {
  initialData?: T; // 初始数据
  immediate?: boolean; // 是否立即执行
  onSuccess?: (data: T) => void; // 成功回调
  onError?: (error: any) => void; // 失败回调
  transform?: (data: any) => T; // 数据转换
  successMessage?: string; // 成功提示
  errorMessage?: string; // 失败提示
}

/**
 * 核心API组合式函数
 * @param apiFn - 要调用的API函数 (例如: getTenantList)
 * @param options - 配置选项
 */
export function useApi<T = any>(
  apiFn: (...args: any[]) => Promise<any>,
  options: UseApiOptions<T> = {}
) {
  const state = reactive<ApiState<T>>({
    data: options.initialData || null,
    loading: false,
    error: null,
    success: false,
  });

  // 执行API调用
  const execute = async (...args: any[]) => {
    state.loading = true;
    state.error = null;
    state.success = false;

    try {
      let response = await apiFn(...args);
      
      // 如果API函数返回了 { data } 结构，则解构
      if (response && response.data !== undefined) {
        response = response.data;
      }

      // 数据转换
      if (options.transform) {
        response = options.transform(response);
      }

      state.data = response;
      state.success = true;

      // 成功提示
      if (options.successMessage) {
        ElMessage.success(options.successMessage);
      }
      
      // 成功回调
      options.onSuccess?.(response);
      
      return response;
    } catch (error: any) {
      state.error = error;

      // [测谎仪] 打印详细错误信息
      console.error('[useApi] Caught an error:', {
        errorObject: error,
        errorName: error.name,
        errorMessage: error.message,
        errorStack: error.stack,
        apiFunctionName: apiFn.name,
      });
      
      // 失败提示
      const message = options.errorMessage || error.message || '操作失败';
      ElMessage.error(message);
      
      // 失败回调
      options.onError?.(error);

      // 重新抛出错误，以便上层可以捕获
      throw error;
    } finally {
      state.loading = false;
    }
  };

  // 如果设置了 immediate，则立即执行
  if (options.immediate) {
    // @ts-ignore
    execute();
  }

  return {
    ...toRefs(state),
    execute,
    isLoading: computed(() => state.loading),
    hasError: computed(() => !!state.error),
  };
}

/**
 * useApi 的快捷别名
 */
export const useRequest = useApi;

/**
 * 用于处理分页列表的API组合式函数
 * @param apiFn - 获取列表的API函数
 * @param options - 配置选项
 */
export function useApiList<T = any>(
  apiFn: (params: any) => Promise<{ items: T[], total: number }>,
  options: UseApiOptions<{ items: T[], total: number }> & {
    params?: MaybeRef<Record<string, any>>;
  } = {}
) {
  const pagination = reactive({
    page: 1,
    pageSize: 10,
    total: 0,
  });

  const allParams = computed(() => ({
    ...toValue(options.params),
    page: pagination.page,
    pageSize: pagination.pageSize,
  }));

  const { data, loading, error, success, execute } = useApi(
    () => apiFn(allParams.value),
    {
      ...options,
      transform: (response) => {
        if (response && Array.isArray(response.items)) {
          pagination.total = response.total;
          return response.items;
        }
        return [];
      },
    }
  );

  const refresh = () => execute();

  const search = () => {
    pagination.page = 1;
    return execute();
  };

  return {
    list: data,
    loading,
    error,
    success,
    pagination,
    refresh,
    search,
  };
}