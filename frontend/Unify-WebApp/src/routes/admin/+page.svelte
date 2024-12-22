<!-- src/routes/admin/+page.svelte -->
<script lang="ts">
    import { user } from '$lib/stores/user';
    import { getAllUsers } from '$lib/api/User/UserRequests';
    import { onMount } from 'svelte';
	import { addUserRole, deleteUserRole } from '$lib/api/Admin/Users/AdminUserRequests';

    let users: Array<{
        id: string;
        email: string;
        firstName: string;
        lastName: string;
        roles: string[];
    }> = [];
    let error = '';
    let loading = true;

    const loadUsers = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            users = await getAllUsers(token);
            loading = false;
        } catch (err) {
            error = (err as Error).message;
            loading = false;
        }
    };

    const handleRoleChange = async (userId: string, role: string, isAdd: boolean) => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');

            if (isAdd) {
                await addUserRole({ userId, role }, token);
            } else {
                await deleteUserRole({ userId, role }, token);
            }
            await loadUsers(); // Refresh list
        } catch (err) {
            error = (err as Error).message;
        }
    };

    onMount(loadUsers);
</script>

<div class="container mt-4">
    <h1 class="mb-4">User Management</h1>

    {#if error}
        <div class="alert alert-danger" role="alert">{error}</div>
    {/if}

    {#if loading}
        <div class="d-flex justify-content-center">
            <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
    {:else}
        <div class="table-responsive">
            <table class="table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Roles</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {#each users as user}
                        <tr>
                            <td>{user.firstName} {user.lastName}</td>
                            <td>{user.email}</td>
                            <td>{user.roles.join(', ')}</td>
                            <td>
                                {#if !user.roles.includes('Student')}
                                    <button 
                                        class="btn btn-sm btn-outline-primary me-2"
                                        on:click={() => handleRoleChange(user.id, 'Student', true)}>
                                        Add Student Role
                                    </button>
                                {:else}
                                    <button 
                                        class="btn btn-sm btn-outline-danger me-2"
                                        on:click={() => handleRoleChange(user.id, 'Student', false)}>
                                        Remove Student Role
                                    </button>
                                {/if}

                                {#if !user.roles.includes('Lecturer')}
                                    <button 
                                        class="btn btn-sm btn-outline-primary"
                                        on:click={() => handleRoleChange(user.id, 'Lecturer', true)}>
                                        Add Lecturer Role
                                    </button>
                                {:else}
                                    <button 
                                        class="btn btn-sm btn-outline-danger"
                                        on:click={() => handleRoleChange(user.id, 'Lecturer', false)}>
                                        Remove Lecturer Role
                                    </button>
                                {/if}
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    {/if}
</div>