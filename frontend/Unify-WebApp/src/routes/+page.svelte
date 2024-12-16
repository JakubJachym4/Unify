<script lang="ts">
  import { user } from '$lib/stores/user';
</script>

<div class="container d-flex align-self-stretch align-items-center justify-content-around">
  <div class="text-center mt-lg-5">
    <h1 class="mb-4">Welcome to Unify</h1>
    <div class="d-flex flex-column align-items-center">
      {#if $user?.roles.length === 1 && $user.roles[0] === 'Registered'}
        <h1 class="text-center">
          {$user.firstName}, your account is not activated yet.
          <br/>
          Your user ID is <span class="fw-bold">{$user.id}</span> please contact the university administration to activate your account.
        </h1>
      {:else}
        Please <a href="/login" class="text-primary">log in</a> to access the application.
      {/if}
    </div>
  </div>
</div>
