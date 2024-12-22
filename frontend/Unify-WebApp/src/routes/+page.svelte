<script lang="ts">
	import { browser } from '$app/environment';
	import { goto } from '$app/navigation';
  import { user } from '$lib/stores/user';
	import { redirect } from '@sveltejs/kit';

  const redirectToAdmin = () => {
    if (browser){
      goto('/admin');
    }
    else {
      redirect(302, '/admin');
    }
  }
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
        {redirectToAdmin()}
      {/if}
    </div>
  </div>
</div>
