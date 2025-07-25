import { ref, onMounted } from 'vue';

export function useTheme() {
  const currentTheme = ref('light');
  
  // 从本地存储加载主题
  onMounted(() => {
    const savedTheme = localStorage.getItem('appTheme') || 'light';
    setTheme(savedTheme);
  });
  
  // 设置主题
  const setTheme = (theme) => {
    currentTheme.value = theme;
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('appTheme', theme);
  };
  
  return { currentTheme, setTheme };
} 