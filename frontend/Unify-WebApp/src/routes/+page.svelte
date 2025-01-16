<script lang="ts">
	import { redirect } from '@sveltejs/kit';
	import { goto } from '$app/navigation';
  import { user } from '$lib/stores/user';
	import { browser } from '$app/environment';

  const redirectTo = (url: string) => {
    if(browser){
      goto(url);
    }
    else{
      throw redirect(301, url);
  }
}

</script>

<div class="container d-flex align-self-stretch align-items-center justify-content-around">
  <div class="text-center mt-lg-5">
    <h1 class="mb-4">Welcome to Unify</h1>
    <div class="d-flex flex-column align-items-center">
        <h1 class="text-center">
          {#if $user && $user.roles.includes('Administrator')}
            {redirectTo('/admin')}

          {:else if $user && $user.roles.includes('Lecturer')}
            {redirectTo('/lecturer')}
          {:else if $user && $user.roles.includes('Student')}
            {redirectTo('/student')}
          {:else if $user}
            {$user.firstName}, your account is not activated yet.
            <br/>
            Your user ID is <span class="fw-bold">{$user.id}</span> please contact the university administration to activate your account.
          {:else}
            {redirectTo('/login')}
          {/if}
           </h1>
    </div>
  </div>
</div>
