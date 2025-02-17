<script lang="ts">
    import { onMount } from 'svelte';
    import type { AcademicLocation, Faculty } from '$lib/types/university';
    import { locationsStore, facultiesStore } from '$lib/stores/university';
	import type { ApiRequestError } from '$lib/api/apiError';
    
    let locations: AcademicLocation[] = [];
    let faculties: Faculty[] = [];
    let loading = true;
    let error = '';
    let searchTerm = '';
    let selectedFaculty = '';
    let selectedBuilding = '';
    let sortKey: 'building' | 'faculty' | 'floor' = 'building';

    const loadData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            
            await Promise.all([
                locationsStore.load(token),
                facultiesStore.load(token)
            ]);
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    $: buildings = [...new Set(locations
        .filter(l => !l.online)
        .map(l => l.building))];

    $: filteredLocations = locations
        .filter(location => !location.online)
        .filter(location => {
            const matchesSearch = searchTerm === '' || 
                location.building.toLowerCase().includes(searchTerm.toLowerCase()) ||
                location.doorNumber.toLowerCase().includes(searchTerm.toLowerCase());
            const matchesFaculty = selectedFaculty === '' || location.facultyId === selectedFaculty;
            const matchesBuilding = selectedBuilding === '' || location.building === selectedBuilding;
            return matchesSearch && matchesFaculty && matchesBuilding;
        })
        .sort((a, b) => {
            switch(sortKey) {
                case 'building':
                    return a.building.localeCompare(b.building);
                case 'faculty':
                    const facultyA = faculties.find(f => f.id === a.facultyId)?.name || '';
                    const facultyB = faculties.find(f => f.id === b.facultyId)?.name || '';
                    return facultyA.localeCompare(facultyB);
                case 'floor':
                    return (a.floor || 0) - (b.floor || 0);
                default:
                    return 0;
            }
        });

    onMount(() => {
        const unsubLocations = locationsStore.subscribe(value => locations = value);
        const unsubFaculties = facultiesStore.subscribe(value => faculties = value);
        loadData();
        return () => {
            unsubLocations();
            unsubFaculties();
        };
    });
</script>

<div class="container mt-4">
    <h2>Physical Locations</h2>

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
                    <div class="col-md-4">
                        <input
                            type="search"
                            class="form-control"
                            placeholder="Search locations..."
                            bind:value={searchTerm}
                        />
                    </div>
                    <div class="col-md-3">
                        <select class="form-select" bind:value={selectedFaculty}>
                            <option value="">All Faculties</option>
                            {#each faculties as faculty}
                                <option value={faculty.id}>{faculty.name}</option>
                            {/each}
                        </select>
                    </div>
                    <div class="col-md-3">
                        <select class="form-select" bind:value={selectedBuilding}>
                            <option value="">All Buildings</option>
                            {#each buildings as building}
                                <option value={building}>{building}</option>
                            {/each}
                        </select>
                    </div>
                    <div class="col-md-2">
                        <select class="form-select" bind:value={sortKey}>
                            <option value="building">Sort by Building</option>
                            <option value="faculty">Sort by Faculty</option>
                            <option value="floor">Sort by Floor</option>
                        </select>
                    </div>
                </div>
            </div>
        </div>

        <div class="table-responsive">
            <table class="table">
                <thead>
                    <tr>
                        <th>Building</th>
                        <th>Floor</th>
                        <th>Room</th>
                        <th>Faculty</th>
                        <th>Street</th>
                    </tr>
                </thead>
                <tbody>
                    {#each filteredLocations as location}
                        <tr>
                            <td>{location.building}</td>
                            <td>{location.floor}</td>
                            <td>{location.doorNumber}</td>
                            <td>{faculties.find(f => f.id === location.facultyId)?.name || '-'}</td>
                            <td>{location.street}</td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    {/if}
</div>