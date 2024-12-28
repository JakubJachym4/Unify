<script lang="ts">
    import { goto } from '$app/navigation';
    import { user } from '$lib/stores/user';
    import { logInUser, getUserData, registerUser } from '$lib/api/User/UserRequests';
    import { universityInformation } from '$lib/stores/university';
    import { globalUsers } from '$lib/stores/globalUsers';
    import type { ApiRequestError } from '$lib/api/apiError';

    let isRegistering = false;
    let email = '';
    let password = '';
    let firstName = '';
    let lastName = '';
    let error = '';
    $: universityName = $universityInformation?.name ?? '';

    const handleSubmit = async () => {
        error = '';
        try {
            if (isRegistering) {
                await registerUser({ email, password, firstName, lastName });
                isRegistering = false; // Switch to login after registration
            } else {
                const data = await logInUser({ email, password });
                localStorage.setItem('token', data.accessToken);
                document.cookie = `user=true; Path=/; SameSite=Strict;`;
                const userData = await getUserData(data.accessToken);
                user.set(userData);
                await globalUsers.refresh(); // Refresh global users
                goto('/');
            }
        } catch (err) {
            console.log(err);
            error = (err as ApiRequestError).details;
        }
    };

    const toggleRegistering = () => {
        isRegistering = !isRegistering;
        error = '';
    };
</script>

<div class="container d-flex flex-column justify-content-center align-items-center vh-100">
    <h1 class="text-center mb-4 user-select-none">{universityName}</h1>
    <div class="card p-4 shadow login-card" style="width: 24rem;">
        <form on:submit|preventDefault={handleSubmit} class="mb-3">
            {#if isRegistering}
                <div class="mb-3">
                    <label for="firstName" class="form-label">First Name</label>
                    <input type="text" id="firstName" bind:value={firstName} class="form-control" required />
                </div>
                <div class="mb-3">
                    <label for="lastName" class="form-label">Last Name</label>
                    <input type="text" id="lastName" bind:value={lastName} class="form-control" required />
                </div>
            {/if}
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
            <button type="submit" class="btn btn-primary w-100">
                {isRegistering ? 'Register' : 'Login'}
            </button>
        </form>
        <div class="btn-group">
            <button 
                class="btn {!isRegistering ? 'btn-primary' : 'btn-outline-primary'}" 
                on:click={toggleRegistering}>
                Login
            </button>
            <button 
                class="btn {isRegistering ? 'btn-primary' : 'btn-outline-primary'}"
                on:click={toggleRegistering}>
                Register
            </button>
        </div>
    </div>
</div>

<style>
    h1 {
        color: #3b4b61;
        font-weight: 600;
        font-size: 4rem;
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