<script lang="ts">
  import { user } from '$lib/stores/user';
</script>

<div class="container mt-5">
  <div class="text-center">
    <h1 class="mb-4">Welcome to the University App</h1>
    <p class="lead">
      {#if $user}
        You are logged in as <strong>{$user.firstName} {$user.lastName}</strong>.
      {:else}
        Please <a href="/login" class="text-primary">log in</a> to access the application.
      {/if}
    </p>
  </div>
</div>
