<script lang="ts">
  import 'bootstrap/dist/css/bootstrap.min.css';
  import Navbar from '$lib/components/navbar/Navbar.svelte';
  import Footer from '$lib/components/footer/Footer.svelte';
	import { onMount } from 'svelte';
  import { Load } from '$lib/start';

  
  onMount(async () => await Load())
</script>

<div class="d-flex flex-column min-vh-100">
<Navbar />
<main class="flex-grow-1">
  <slot />
</main>
<Footer />
</div>