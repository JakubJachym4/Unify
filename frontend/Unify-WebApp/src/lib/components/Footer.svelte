<script lang="ts">
    import { unifyLogo } from '$lib/constants/literals'
</script>

<footer class="bg-light-grey text-center text-muted py-3">
  <small>&copy; {new Date().getFullYear()} {unifyLogo}.    All Rights Reserved.</small>
</footer>

<style>
  .bg-light-grey {
    background-color: #f1f1f1; /* Light grey for footer */
  }
</style>
