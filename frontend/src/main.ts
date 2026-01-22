import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { VueQueryPlugin } from '@tanstack/vue-query'
import App from './App.vue'
import router from './router'
import './assets/styles/main.css'
import { appConfig } from '@/config/app.config'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(VueQueryPlugin, {
  queryClientConfig: {
    defaultOptions: {
      queries: {
        refetchOnWindowFocus: false,
        retry: appConfig.api.retries,
        staleTime: appConfig.cache.staleTime
      }
    }
  }
})

app.mount('#app')
