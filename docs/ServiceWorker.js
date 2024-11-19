const cacheName = "PolygonUs-SpaceShooter-0.3.3";
const contentToCache = [
    "Build/7f46c4e10add95d60a7c6deacf727032.loader.js",
    "Build/3fc20a04c3106e559c8ea8586bb34e1f.framework.js",
    "Build/d1ee69fd965746c0a28849f21126f6b6.data",
    "Build/ed2ee966ea922d50754d163f9fe22253.wasm",
    "TemplateData/style.css"

];

self.addEventListener('install', function (e) {
    console.log('[Service Worker] Install');
    
    e.waitUntil((async function () {
      const cache = await caches.open(cacheName);
      console.log('[Service Worker] Caching all: app shell and content');
      await cache.addAll(contentToCache);
    })());
});

self.addEventListener('fetch', function (e) {
    e.respondWith((async function () {
      let response = await caches.match(e.request);
      console.log(`[Service Worker] Fetching resource: ${e.request.url}`);
      if (response) { return response; }

      response = await fetch(e.request);
      const cache = await caches.open(cacheName);
      console.log(`[Service Worker] Caching new resource: ${e.request.url}`);
      cache.put(e.request, response.clone());
      return response;
    })());
});
