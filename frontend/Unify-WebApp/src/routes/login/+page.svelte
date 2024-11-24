<script lang="ts">
	import { goto } from '$app/navigation';
	import { user } from '$lib/stores/user';

	let email = '';
	let password = '';
	let error = '';

	const login = async () => {
		if (email === 'user@example.com' && password === 'password') {
			user.set({ id: '1', name: 'John Doe', email });
			goto('/'); // Redirect to the homepage
		} else {
			error = 'Invalid email or password.';
		}
	};
</script>

<div class="container d-flex justify-content-center align-items-center vh-100">
	<div class="card p-4 shadow" style="width: 24rem;">
		<h2 class="card-title text-center">Login</h2>
		<form on:submit|preventDefault={login}>
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
