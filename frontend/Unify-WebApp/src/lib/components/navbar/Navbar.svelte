<script lang="ts">
  import { goto } from '$app/navigation';
  import { user } from '$lib/stores/user';
  import { unifyLogo } from '$lib/constants/literals';
	import { universityInformationStore } from '$lib/stores/university';
	import { logOutUser } from '$lib/api/User/UserRequests';

  $: universityName = $universityInformationStore?.name ?? '';

  const logout = async () => {
    const token = localStorage.getItem('token');
    if (token){
      await logOutUser(token); 
      localStorage.removeItem('token');
    }
    
    document.cookie = 'user=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT; SameSite=Strict;';
    user.set(null);
    
    goto('/login');
  };
</script>

<nav class="navbar navbar-expand-lg navbar-dark bg-dark-grey text-light py-0 user-select-none">
  <div class="container-fluid d-flex align-items-center px-3">
    <!-- Unify Logo -->
    <a class="navbar-brand d-flex align-items-center me-3" href="/">
      <span class="text-primary fw-semibold text-white display-4 mb-0">{unifyLogo}</span>
    </a>

    <!-- University Name -->
    <span class="navbar-text text-center mx-auto fs-4 fw-semibold">
      {universityName}
    </span> 

    <!-- Logout Button -->
    {#if $user}
      <div class="me-3">
        <button class="btn btn-outline-danger btn-md" on:click={logout}>Logout</button>
      </div>
    {/if}
  </div>
</nav>

<style>
  .bg-dark-grey {
    background-color: #3b4b61; /* Greyish navy blue */
  }

  .navbar-brand {
    margin-left: 0; /* Ensures the logo starts at the very left edge */
  }

  .display-4 {
    font-size: 2.5rem; /* Larger font size for the logo */
  }

  .navbar {
    height: auto; /* Adjusts height dynamically to fit the content */
  }

  .container-fluid {
    padding-left: 0;
    padding-right: 0;
  }
</style>
