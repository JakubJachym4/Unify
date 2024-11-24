<script lang="ts">
    import { user } from '$lib/stores/user';
    import '../app.css';
</script>


<div class="container mt-5">
    <h1>Welcome to the University App</h1>
    <p>
        {#if $user}You are logged in as {$user.name}.
        {:else}Please log in to access the application.
        {/if}</p>
</div>
