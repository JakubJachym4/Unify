<script lang="ts">
	import { goto } from '$app/navigation';
  import { user } from '$lib/stores/user';

</script>

<div class="container d-flex align-self-stretch align-items-center justify-content-around">
  <div class="text-center mt-lg-5">
    <h1 class="mb-4">Welcome to Unify</h1>
    <div class="d-flex flex-column align-items-center">
        <h1 class="text-center">
          {#if $user && $user.roles.includes('Administrator')}
            {goto('/admin')}

          {:else if $user && $user.roles.includes('Lecturer')}
            {goto('/lecturer')}
          {:else if $user && $user.roles.includes('Student')}
            {goto('/student')}
          {:else if $user}
            {$user.firstName}, your account is not activated yet.
            <br/>
            Your user ID is <span class="fw-bold">{$user.id}</span> please contact the university administration to activate your account.
          {:else}
            {goto('/login')}
          {/if}
           </h1>
    </div>
  </div>
</div>
