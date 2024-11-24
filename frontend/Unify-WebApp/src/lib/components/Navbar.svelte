<script lang="ts">
    import { goto } from '$app/navigation';
    import { user } from '$lib/stores/user';

    const logout = () => {
        document.cookie = 'user=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
        user.set(null);
        goto('/login');
    };
</script>

<nav class="navbar navbar-expand-lg navbar-light bg-light">
    <div class="container-fluid">
        <a class="navbar-brand" href="/">University App</a>
        <div class="d-flex">
            {#if $user}
                <span class="navbar-text me-3">Welcome, {$user.name}</span>
                <button class="btn btn-outline-danger" on:click={logout}>Logout</button>
            {:else}
                <a class="btn btn-primary" href="/login">Login</a>
            {/if}
        </div>
    </div>
</nav>
