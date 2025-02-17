<script lang="ts">
  import 'bootstrap/dist/css/bootstrap.min.css';
  import Navbar from '$lib/components/navbar/Navbar.svelte';
  import Footer from '$lib/components/footer/Footer.svelte';
  import { onMount } from 'svelte';
  import { Load } from '$lib/start';
  
  onMount(async () => await Load())
</script>

<div class="app-container">
  <Navbar />
  <main class="main-content">
    <slot />
  </main>
  <Footer />
</div>

<style>
  .app-container {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
  }

  .main-content {
    flex: 1;
    overflow-y: auto;
    position: relative;
    padding-bottom: 1rem; /* Add some bottom padding to prevent content from touching footer */
  }
</style>