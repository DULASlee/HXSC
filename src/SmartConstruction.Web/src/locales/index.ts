// =============================================
// 国际化配置
// =============================================
import { createI18n } from 'vue-i18n'
import zhCN from './zh-CN'
import en from './en'
import zhCNJson from './zh-CN.json'
import enUSJson from './en-US.json'
import jaJPJson from './ja-JP.json'

// 合并新旧翻译
const zhCNMessages = {
  ...zhCN,
  ...zhCNJson
}

const enMessages = {
  ...en,
  ...enUSJson
}

const jaMessages = {
  ...jaJPJson
}

const messages = {
  'zh-CN': zhCNMessages,
  'en-US': enMessages,
  'ja-JP': jaMessages,
  // 别名支持
  'zh': zhCNMessages,
  'en': enMessages,
  'ja': jaMessages
}

// 获取浏览器语言
function getLanguage() {
  const chooseLanguage = localStorage.getItem('language')
  if (chooseLanguage) return chooseLanguage

  // 如果没有选择语言
  const language = navigator.language.toLowerCase()
  const locales = Object.keys(messages)
  
  // 精确匹配
  if (messages[language]) {
    return language
  }
  
  // 模糊匹配
  for (const locale of locales) {
    if (language.indexOf(locale.toLowerCase()) > -1) {
      return locale
    }
  }
  
  // 根据语言前缀匹配
  const languagePrefix = language.split('-')[0]
  const prefixMap: Record<string, string> = {
    'zh': 'zh-CN',
    'en': 'en-US', 
    'ja': 'ja-JP'
  }
  
  if (prefixMap[languagePrefix]) {
    return prefixMap[languagePrefix]
  }
  
  return 'zh-CN' // 默认中文
}

const i18n = createI18n({
  legacy: false,
  locale: getLanguage(),
  fallbackLocale: 'zh-CN',
  messages,
  globalInjection: true,
  silentTranslationWarn: true
})

export default i18n