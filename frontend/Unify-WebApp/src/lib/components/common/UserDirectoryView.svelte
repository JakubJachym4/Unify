<script lang="ts">
    import { onMount } from 'svelte';
    import type { UserResponse } from '$lib/api/User/UserRequests';
    import { globalUsers } from '$lib/stores/globalUsers';
	import type { ApiRequestError } from '$lib/api/apiError';

    let users: UserResponse[] = [];
    let loading = true;
    let error = '';
    let searchTerm = '';
    let roleFilter: 'all' | 'student' | 'lecturer' | 'admin' = 'all';

    const loadData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await globalUsers.refresh();
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const getDisplayRoles = (roles: string[]): string[] => {
        return roles.filter(role => role !== 'Registered');
    };

    $: filteredUsers = users.filter(user => {
        const matchesSearch = 
            user.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
            user.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
            user.email.toLowerCase().includes(searchTerm.toLowerCase());
            
        const matchesRole = roleFilter === 'all' || user.roles.includes(roleFilter[0].toUpperCase() + roleFilter.slice(1));
        
        return matchesSearch && matchesRole;
    });

    $: userStats = {
        total: users.length,
        students: users.filter(u => u.roles.includes('Student')).length,
        lecturers: users.filter(u => u.roles.includes('Lecturer')).length,
        admins: users.filter(u => u.roles.includes('Admin')).length
    };

    onMount(() => {
        const unsubUsers = globalUsers.subscribe(value => users = value);
        loadData();
        return () => unsubUsers();
    });
</script>

<div class="container mt-4">
    <h2>User Directory</h2>

    {#if error}
        <div class="alert alert-danger">{error}</div>
    {/if}

    {#if loading}
        <div class="d-flex justify-content-center">
            <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
    {:else}
        <div class="card mb-4">
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-md-8">
                        <input
                            type="search"
                            class="form-control"
                            placeholder="Search users..."
                            bind:value={searchTerm}
                        />
                    </div>
                    <div class="col-md-4">
                        <select class="form-select" bind:value={roleFilter}>
                            <option value="all">All Users</option>
                            <option value="student">Students</option>
                            <option value="lecturer">Lecturers</option>
                            <option value="admin">Administrators</option>
                        </select>
                    </div>
                </div>
            </div>
        </div>

        <div class="row mb-4">
            <div class="col-md-3">
                <div class="card text-center">
                    <div class="card-body">
                        <h5 class="card-title">Total Users</h5>
                        <p class="card-text display-6">{userStats.total}</p>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card text-center">
                    <div class="card-body">
                        <h5 class="card-title">Students</h5>
                        <p class="card-text display-6">{userStats.students}</p>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card text-center">
                    <div class="card-body">
                        <h5 class="card-title">Lecturers</h5>
                        <p class="card-text display-6">{userStats.lecturers}</p>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card text-center">
                    <div class="card-body">
                        <h5 class="card-title">Admins</h5>
                        <p class="card-text display-6">{userStats.admins}</p>
                    </div>
                </div>
            </div>
        </div>

        <div class="table-responsive">
            <table class="table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Roles</th>
                    </tr>
                </thead>
                <tbody>
                    {#each filteredUsers as user}
                        <tr>
                            <td>{user.firstName} {user.lastName}</td>
                            <td>{user.email}</td>
                            <td>
                                {#each getDisplayRoles(user.roles) as role}
                                    <span class="badge bg-{role === 'Admin' ? 'danger' : role === 'Lecturer' ? 'primary' : 'success'} me-1">
                                        {role}
                                    </span>
                                {/each}
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    {/if}
</div>