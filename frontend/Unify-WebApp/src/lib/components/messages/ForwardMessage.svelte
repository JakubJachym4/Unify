<script lang="ts">
    import { onMount, onDestroy } from 'svelte';
    import { forwardMessage } from '$lib/api/Messages/MessagesRequests';
    import type { UserResponse } from '$lib/api/User/UserRequests';
    import { globalUsers } from '$lib/stores/globalUsers';
    import { get } from 'svelte/store';
    import { messagesStore } from '$lib/stores/messages';
    import type { ApiRequestError } from '$lib/api/apiError';
// Add event dispatcher
    import { createEventDispatcher } from 'svelte';
	import { goto } from '$app/navigation';
    const dispatch = createEventDispatcher<{recipientsSelected: string[]}>();

    export let show = false;
    export let onClose: () => void;
    export let modalLevel = 1;
    export let messageId: string | undefined = undefined;
    export let shouldDispatch = false;

    let error = '';
    let forwardRecipients: string[] = [];
    let availableUsers: UserResponse[] = [];
    let searchTerm = '';
    let showDropdown = false;
    let dropdownRef: HTMLDivElement;

    $: filteredUsers = availableUsers.filter(user => 
        !forwardRecipients.includes(user.id) &&
        (user.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
         user.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
         user.email.toLowerCase().includes(searchTerm.toLowerCase()))
    );

    const handleForward = async () => {
        if (forwardRecipients.length === 0) {
            error = 'Please select recipients for forwarding';
            return;
        }

        if(shouldDispatch){
            dispatch('recipientsSelected', forwardRecipients);
            return;
        }
        


        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            
            await forwardMessage({
                originalMessageId: messageId,
                newRecipientsIds: forwardRecipients
            }, token);

            const lastWeek = new Date();
            lastWeek.setDate(lastWeek.getDate() - 7);
            const date = `${lastWeek.getFullYear()}-${lastWeek.getMonth() + 1}-${lastWeek.getDate()}`;
            await messagesStore.refresh(date);
            goto('/');
            onClose();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleClickOutside = (event: MouseEvent) => {
        if (dropdownRef && !dropdownRef.contains(event.target as Node)) {
            showDropdown = false;
        }
    };

    onMount(() => {
        const users = get(globalUsers);
        availableUsers = users;
        document.addEventListener('click', handleClickOutside);
    });

    onDestroy(() => {
        document.removeEventListener('click', handleClickOutside);
    });
</script>

{#if show}
<div class="modal show d-block" tabindex="-1" style="z-index: {1050 + modalLevel}">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Forward Message</h5>
                <button type="button" class="btn-close" on:click={onClose}></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label class="form-label">Select Recipients to Forward To:</label>
                    <div class="dropdown position-relative" bind:this={dropdownRef}>
                        <input 
                            type="text" 
                            class="form-control" 
                            placeholder="Search recipients..."
                            bind:value={searchTerm}
                            on:focus={() => showDropdown = true}
                        >
                        {#if showDropdown && filteredUsers.length > 0}
                            <div class="dropdown-menu show">
                                {#each filteredUsers as user}
                                    <button
                                        class="dropdown-item"
                                        on:click={() => {
                                            if (!forwardRecipients.includes(user.id)) {
                                                forwardRecipients = [...forwardRecipients, user.id];
                                            }
                                            searchTerm = '';
                                        }}
                                    >
                                        {user.firstName} {user.lastName}
                                    </button>
                                {/each}
                            </div>
                        {/if}
                    </div>
                    <div class="selected-recipients mt-2">
                        {#each forwardRecipients as recipientId}
                            {#each availableUsers.filter(u => u.id === recipientId) as user}
                                <span class="badge bg-primary me-2">
                                    {user.firstName} {user.lastName}
                                    <button 
                                        type="button" 
                                        class="btn-close btn-close-white ms-1"
                                        on:click={() => {
                                            forwardRecipients = forwardRecipients.filter(id => id !== user.id);
                                        }}
                                    ></button>
                                </span>
                            {/each}
                        {/each}
                    </div>
                </div>
                {#if error}
                    <div class="alert alert-danger">{error}</div>
                {/if}
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" on:click={onClose}>Cancel</button>
                <button type="button" class="btn btn-primary" on:click={handleForward}>Forward Message</button>
            </div>
        </div>
    </div>
</div>
<div class="modal-backdrop show" style="z-index: {1040 + modalLevel}"></div>
{/if}

<style>
    .modal-backdrop {
        background-color: rgba(0,0,0,0.5);
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
    }

    .dropdown-menu {
        max-height: 200px;
        overflow-y: auto;
    }

    .selected-recipients {
        display: flex;
        flex-wrap: wrap;
        gap: 0.5rem;
    }
</style>