<script lang="ts">
    import { onMount } from 'svelte';
    import type { FieldOfStudy, Specialization } from '$lib/types/university';
    import { fieldOfStudiesStore, specializationsStore } from '$lib/stores/university';
	import type { ApiRequestError } from '$lib/api/apiError';

    let fieldsOfStudy: FieldOfStudy[] = [];
    let specializations: Specialization[] = [];
    let loading = true;
    let error = '';
    let searchTerm = '';
    let expandedFields: string[] = [];

    const loadData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            
            await Promise.all([
                fieldOfStudiesStore.load(token),
                specializationsStore.load(token)
            ]);
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const toggleField = (fieldId: string) => {
        const index = expandedFields.indexOf(fieldId);
        if (index === -1) {
            expandedFields = [...expandedFields, fieldId];
        } else {
            expandedFields = expandedFields.filter(id => id !== fieldId);
        }
    };

    $: filteredFields = fieldsOfStudy.filter(field => 
        field.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        specializations
            .filter(s => s.fieldOfStudyId === field.id)
            .some(s => s.name.toLowerCase().includes(searchTerm.toLowerCase()))
    );

    onMount(() => {
        const unsubFields = fieldOfStudiesStore.subscribe(value => fieldsOfStudy = value);
        const unsubSpecs = specializationsStore.subscribe(value => specializations = value);
        loadData();
        return () => {
            unsubFields();
            unsubSpecs();
        };
    });
</script>

<div class="container mt-4">
    <h2>Academic Structure</h2>

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
                <input
                    type="search"
                    class="form-control"
                    placeholder="Search fields or specializations..."
                    bind:value={searchTerm}
                />
            </div>
        </div>

        <div class="accordion">
            {#each filteredFields as field}
                {@const fieldSpecs = specializations.filter(s => s.fieldOfStudyId === field.id)}
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button
                            class="accordion-button {expandedFields.includes(field.id) ? '' : 'collapsed'}"
                            type="button"
                            on:click={() => toggleField(field.id)}
                        >
                            <strong>{field.name}</strong>
                            <span class="badge bg-secondary ms-2">
                                {fieldSpecs.length} specializations
                            </span>
                        </button>
                    </h2>
                    <div class="accordion-collapse collapse {expandedFields.includes(field.id) ? 'show' : ''}">
                        <div class="accordion-body">
                            <div class="list-group">
                                {#each fieldSpecs as spec}
                                    <div class="list-group-item">
                                        <h6 class="mb-1">{spec.name}</h6>
                                        <p class="mb-1 text-muted">{spec.description}</p>
                                    </div>
                                {/each}
                            </div>
                        </div>
                    </div>
                </div>
            {/each}
        </div>
    {/if}
</div>