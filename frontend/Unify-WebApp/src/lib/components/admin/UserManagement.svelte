<script lang="ts">
    import { onMount } from 'svelte';
    import { getAllUsers, type UserResponse } from '$lib/api/User/UserRequests';
    import { addUserRole, removeUserRole } from '$lib/api/Admin/Users/AdminUserRequests';
    import type { ApiRequestError } from '$lib/api/apiError';
    import { globalUsers } from '$lib/stores/globalUsers';

    let users: UserResponse[] = [];
    let error = '';
    let loading = true;
    let searchTerm = '';

    const loadUsers = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            const fetchedUsers = await getAllUsers(token);
            globalUsers.set(fetchedUsers);
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
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
                await removeUserRole({ userId, role }, token);
            }
            await loadUsers(); // Refresh list
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    $: filteredUsers = users.filter(user => {
        const searchLower = searchTerm.toLowerCase();
        return user.firstName.toLowerCase().includes(searchLower) ||
               user.lastName.toLowerCase().includes(searchLower) ||
               user.email.toLowerCase().includes(searchLower) ||
               user.roles.some(role => role.toLowerCase().includes(searchLower));
    });

    onMount(() => {
        const unsubscribe = globalUsers.subscribe(value => {
            users = value;
        });
        loadUsers();
        return unsubscribe;
    });
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
        <div class="row mb-3">
            <div class="col">
                <input
                    type="search"
                    class="form-control"
                    placeholder="Search users..."
                    bind:value={searchTerm}
                />
            </div>
        </div>
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
                    {#each filteredUsers as user}
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

                                {#if !user.roles.includes('Administrator')}
                                    <button 
                                        class="btn btn-sm btn-outline-primary"
                                        on:click={() => handleRoleChange(user.id, 'Administrator', true)}>
                                        Add Admin Role
                                    </button>
                                {:else}
                                    <button 
                                        class="btn btn-sm btn-outline-danger"
                                        on:click={() => handleRoleChange(user.id, 'Administrator', false)}>
                                        Remove Admin Role
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