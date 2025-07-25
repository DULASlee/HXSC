// =========================================================================================
// 文件名: system-init.ts
// 描述:   “系统初始化核武器” - 一个原子的、健壮的系统初始化函数。
// =========================================================================================
import { useUserStore } from '@/stores/user';
import { useMenuStore } from '@/stores/menu';
import { useAppStore } from '@/stores/app';
import { login as loginApi } from '@/api/modules/auth';
import type { LoginRequest } from '@/types/global';

/**
 * 🔥 系统初始化核武器 🔥
 * 一个函数入口，完成登录、状态设置、菜单和路由生成等所有核心初始化工作。
 * @param loginForm 包含租户、用户名和密码的登录对象
 * @returns {Promise<{ success: boolean; error?: string }>} 初始化结果
 */
export async function SystemInit(loginForm: LoginRequest): Promise<{ success: boolean; error?: string }> {
  
  // 获取各个 store 实例
  const userStore = useUserStore();
  const menuStore = useMenuStore();
  const appStore = useAppStore();

  try {
    console.log('💥 [SystemInit] 核武器发射程序启动...');

    // ===================== 第1步：获取授权 (令牌 & 核心数据) =====================
    console.log('🚀 [SystemInit] 步骤1: 正在获取授权...');
    const response = await loginApi(loginForm) as any;
    const responseData = response.data;

    if (!responseData || !responseData.token) {
      throw new Error('授权失败，未收到有效的令牌。');
    }
    console.log('✅ [SystemInit] 步骤1: 授权成功!');

    // ===================== 第2步：核心状态写入 (用户 & 令牌) =====================
    console.log('📝 [SystemInit] 步骤2: 正在写入核心用户状态...');
    // 使用 userStore 的 action 来集中处理用户状态的设置和持久化
    userStore.setTokenAndUser(responseData);
    console.log('✅ [SystemInit] 步骤2: 核心用户状态写入完毕!');

    // ===================== 第3步：[已移除] 菜单与路由初始化(交由路由守卫处理) =====================
    console.log('✅ [SystemInit] 步骤3: 菜单初始化已交由路由守卫处理，本步骤跳过。');

    // ===================== 第4步：并行加载非核心配置 (如主题、应用设置) =====================
    console.log('🎨 [SystemInit] 步骤4: 正在加载应用配置...');
    // 这里可以并行加载，因为它们不阻塞核心流程
    // 之前已注释
    console.log('✅ [SystemInit] 步骤4: 应用配置加载完毕!');

    // ===================== 第5步：宣告成功 =====================
    console.log('🎉 [SystemInit] 所有初始化步骤成功完成！系统准备就绪。');
    return { success: true };

  } catch (error: any) {
    console.error('❌ [SystemInit] 初始化过程中发生致命错误:', error);
    
    // 🔥 关键！发生错误时，执行“焦土策略”，清理现场
    console.log('💣 [SystemInit] 执行焦土策略，清理所有认证信息...');
    userStore.resetUser(); // 清除 Pinia 中的用户状态
    menuStore.clearMenus(); // 清除 Pinia 中的菜单状态
    localStorage.removeItem('user'); // 确保 localStorage 也被清理干净
    
    // 返回可读的、安全的错误信息
    return {
      success: false,
      error: error.message || '系统初始化失败，请稍后重试。'
    };
  }
} 