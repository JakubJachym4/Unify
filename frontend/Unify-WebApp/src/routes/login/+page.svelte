<script lang="ts">
    import { goto } from '$app/navigation';
    import { user } from '$lib/stores/user';
    import { logInUser, getUserData } from '$lib/api/User/UserRequests';
    import { universityInformation } from '$lib/stores/university';

    let email = '';
    let password = '';
    let error = '';
    $: universityName = $universityInformation?.abbreviation ?? $universityInformation?.name ?? '';

    const handleLogin = async () => {
        try {
            const data = await logInUser({ email, password });
            localStorage.setItem('token', data.accessToken); 


            const userData = await getUserData(data.accessToken);
            console.log('User data:', userData); // Debugging log
            user.set(userData);

            goto('/'); 
        } catch (err) {
            error = (err as Error).message;
            console.error('Login error:', error);
        }
    };
</script>

<div class="container d-flex flex-column justify-content-center align-items-center vh-100">
    <h1 class="text-center mb-5 user-select-none">{universityName}</h1>
    <div class="card p-4 shadow login-card" style="width: 24rem;">
        <h2 class="card-title text-center user-select-none">Login</h2>
        <form on:submit|preventDefault={handleLogin}>
            <div class="mb-3">
                <label for="email" class="form-label">Email address</label>
                <input type="email" id="email" bind:value={email} class="form-control" required />
            </div>
            <div class="mb-3">
                <label for="password" class="form-label">Password</label>
                <input type="password" id="password" bind:value={password} class="form-control" required />
            </div>
            {#if error}
                <div class="alert alert-danger" role="alert">{error}</div>
            {/if}
            <button type="submit" class="btn btn-primary w-100">Login</button>
        </form>
    </div>
</div>

<style>
    h1 {
        color: #3b4b61;
        font-weight: 600;
        font-size: 15rem;
    }

    .login-card {
        width: 20rem;
    }

    @media (max-width: 768px) {
        h1 {
            font-size: 3rem;
        }
    }
</style>