import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from 'node:url'
import { visualizer } from 'rollup-plugin-visualizer'

// Helper function for safe integer parsing
function safeParseInt(value: string | undefined, fallback: number): number {
  if (!value) return fallback
  const parsed = parseInt(value, 10)
  return isNaN(parsed) ? fallback : parsed
}

export default defineConfig(({ mode }) => {
  // Load environment variables
  const env = loadEnv(mode, process.cwd(), '')
  
  // Configuration values from environment with defaults
  const config = {
    api: {
      baseUrl: env.VITE_API_URL || 'https://localhost:7025'
    },
    app: {
      port: safeParseInt(env.VITE_APP_PORT, 3000),
      autoOpenBrowser: env.VITE_AUTO_OPEN_BROWSER === 'true'
    },
    build: {
      outputDir: env.VITE_OUTPUT_DIR || 'dist',
      sourcemap: env.VITE_SOURCEMAP_ENABLED !== 'false',
      bundleAnalyzer: env.VITE_BUNDLE_ANALYZER === 'true',
      statsFilename: env.VITE_STATS_FILENAME || './dist/stats.html'
    }
  }

  return {
    plugins: [
      vue(),
      visualizer({
        filename: config.build.statsFilename,
        open: config.build.bundleAnalyzer,
        gzipSize: true,
        brotliSize: true,
      })
    ],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url))
      }
    },
    server: {
      port: config.app.port,
      open: config.app.autoOpenBrowser,
      cors: true,
      proxy: {
        '/api': {
          target: config.api.baseUrl,
          changeOrigin: true,
          secure: false,  // ✅ Accept self-signed certificates
          ws: true,
          configure: (proxy, _options) => {
            proxy.on('error', (err, req, _res) => {
              console.error('❌ Proxy Error Details:')
              console.error(`   URL: ${req.url}`)
              console.error(`   Error: ${err.message}`)
              console.error(`   Code: ${err.code}`)
            })
            proxy.on('proxyReq', (proxyReq, req, _res) => {
              console.log(`🔀 Proxy Request: ${req.method} ${req.url} → ${config.api.baseUrl}${req.url}`)
            })
            proxy.on('proxyRes', (proxyRes, req, _res) => {
              console.log(`✅ Proxy Response: ${req.url} → Status: ${proxyRes.statusCode}`)
            })
          }
        }
      }
    },
    build: {
      outDir: config.build.outputDir,
      sourcemap: config.build.sourcemap,
      rollupOptions: {
        output: {
          manualChunks: {
            'vendor-vue': ['vue', 'vue-router', 'pinia'],
            'vendor-query': ['@tanstack/vue-query'],
            'vendor-ui': ['@heroicons/vue'],
          }
        }
      }
    }
  }
})
