<script lang="ts">
    import 'bootstrap/dist/css/bootstrap.min.css';
    import Navbar from '$lib/components/Navbar.svelte';
    import Footer from '$lib/components/Footer.svelte';


    let universityName = 'Unify University'

</script>

<div class="d-flex flex-column min-vh-100">
<Navbar {universityName} />
<main class="flex-grow-1">
  <slot />
</main>
<Footer />
</div>