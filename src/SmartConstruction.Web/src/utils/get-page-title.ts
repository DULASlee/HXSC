import { useI18n } from "vue-i18n";

const title = "智慧工地SaaS平台";

export function getPageTitle(pageTitle?: string): string {
  const { t } = useI18n();

  if (pageTitle) {
    const translatedTitle = t(`route.${pageTitle}`);
    return `${translatedTitle} - ${title}`;
  }
  return title;
}
