import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  assetsInclude: ['**/*.md'],
  server: {
    port: parseInt(process.env.PORT ?? "5173"),
    proxy: {
      '/api': {
        target: 
          process.env.services__backend__https__0 ||
          process.env.services__backend__http__0,
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
