<template>
  <component :is="type" v-bind="linkProps(to)">
    <slot />
  </component>
</template>

<script lang="ts">
export default {
  name: 'AppLink'
}
</script>

<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps({
  to: {
    type: String,
    required: true
  }
});

const isExternal = computed(() => /^(https?:|mailto:|tel:)/.test(props.to));

const type = computed(() => {
  if (isExternal.value) {
    return 'a';
  }
  return 'router-link';
});

const linkProps = (to) => {
  if (isExternal.value) {
    return {
      href: to,
      target: '_blank',
      rel: 'noopener'
    };
  }
  return { to };
};
</script> 