<script lang="ts">
	import { sendMessage, type SendMessageRequest } from './../../api/Messages/MessagesRequests';
    import { onMount, onDestroy } from 'svelte';
    import type { UserResponse } from '$lib/api/User/UserRequests';
    import { getUserData } from '$lib/api/User/UserRequests';
	import type { ApiRequestError } from '$lib/api/apiError.js';
	import { api } from '$lib/api/api.js';
    import { globalUsers } from '$lib/stores/globalUsers';
    import { get } from 'svelte/store';
	import { browser } from '$app/environment';
    import { messages } from '$lib/stores/messages';

    export let show = false;
    export let onClose: () => void;

    let title = '';
    let content = '';
    let recipientsIds: string[] = [];
    let attachments: File[] = [];
    let error = '';
    let submitting = false;

    let availableUsers: UserResponse[] = [];
    let searchTerm = '';
    let showDropdown = false;

    let fileInput: HTMLInputElement;

    $: filteredUsers = availableUsers.filter(user => 
        !recipientsIds.includes(user.id) &&
        (user.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
         user.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
         user.email.toLowerCase().includes(searchTerm.toLowerCase()))
    );

    const handleKeydown = (event: KeyboardEvent) => {
        if (event.key === 'Escape' && show) {
            onClose();
        }
    };

    const handleFileInput = (event: Event) => {
        const input = event.target as HTMLInputElement;
        if (input.files) {
            attachments = Array.from(input.files);
        }
    };

    const removeAttachment = (index: number) => {
        attachments = attachments.filter((_, i) => i !== index);
        if (fileInput) {
            fileInput.value = ''; // Reset file input
        }
    };

    const handleSubmit = async () => {
        if (!title || !content || recipientsIds.length === 0) {
            error = 'Please fill all required fields';
            return;
        }

        error = '';
        submitting = true;

        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            
            const request: SendMessageRequest = {
                title,
                content,
                recipientsIds,
                attachments
            };
            
            await sendMessage(request, token);
            
            // Refresh messages list
            const lastWeek = new Date(); 
            lastWeek.setDate(lastWeek.getDate() - 7);
            const date = `${lastWeek.getFullYear()}-${lastWeek.getMonth() + 1}-${lastWeek.getDate()}`;
            await messages.refresh(date);
            
            // Dispatch success event
            dispatchEvent(new CustomEvent('messageSent'));
            onClose();
        } catch (err) {
            error = (err as ApiRequestError).details;
        } finally {
            submitting = false;
        }
    };

    onMount(() => {
        if(browser){
            window.addEventListener('keydown', handleKeydown);
            const users = get(globalUsers);
            availableUsers = users;
        }

    });

    onDestroy(() => {
        if(browser){
            window.removeEventListener('keydown', handleKeydown);
        }
    });

    const removeRecipient = (id: string) => {
        recipientsIds = recipientsIds.filter(recipientId => recipientId !== id);
    };

    const addRecipient = (user: UserResponse) => {
        recipientsIds = [...recipientsIds, user.id];
        searchTerm = '';
        showDropdown = false;
    };
</script>

{#if show}
<div class="modal show d-block" tabindex="-1">
    <div class="modal-dialog modal-xl">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">New Message</h5>
                <!-- svelte-ignore a11y_consider_explicit_label -->
                <button type="button" class="btn-close" on:click={onClose}></button>
            </div>
            <div class="modal-body">
                <form on:submit|preventDefault={handleSubmit}>
                    <div class="mb-3">
                        <label for="title" class="form-label">Title</label>
                        <input type="text" class="form-control" id="title" bind:value={title} required>
                    </div>
                    <div class="mb-3 recipients-container">
                        <label for="recipients" class="form-label">Recipients</label>
                        <div class="selected-recipients mb-2">
                            {#each recipientsIds as recipientId}
                                {#each availableUsers.filter(u => u.id === recipientId) as user}
                                    <span class="badge bg-primary me-2 mb-2">
                                        {user.firstName} {user.lastName}
                                        <!-- svelte-ignore a11y_consider_explicit_label -->
                                        <button 
                                            type="button" 
                                            class="btn-close btn-close-white ms-2"
                                            on:click={() => removeRecipient(user.id)}
                                        ></button>
                                    </span>
                                {/each}
                            {/each}
                        </div>
                        <div class="position-relative">
                            <input 
                                type="text" 
                                class="form-control" 
                                placeholder="Search recipients..."
                                bind:value={searchTerm}
                                on:focus={() => showDropdown = true}
                            >
                            {#if showDropdown && filteredUsers.length > 0}
                                <div class="recipients-dropdown">
                                    {#each filteredUsers as user}
                                        <!-- svelte-ignore a11y_click_events_have_key_events -->
                                        <!-- svelte-ignore a11y_no_static_element_interactions -->
                                        <div 
                                            class="recipient-option"
                                            on:click={() => addRecipient(user)}
                                        >
                                            {user.firstName} {user.lastName} ({user.email})
                                        </div>
                                    {/each}
                                </div>
                            {/if}
                        </div>
                    </div>
                    <div class="mb-3">
                        <label for="content" class="form-label">Message</label>
                        <textarea class="form-control" id="content" rows="5" bind:value={content} required></textarea>
                    </div>
                    <div class="mb-3">
                        <label for="attachments" class="form-label">Attachments</label>
                        <input 
                            type="file" 
                            class="form-control" 
                            id="attachments" 
                            multiple
                            bind:this={fileInput}
                            on:change={handleFileInput}
                        >
                        {#if attachments.length > 0}
                            <div class="attachment-list mt-2">
                                {#each attachments as file, index}
                                    <div class="attachment-item">
                                        <span>{file.name}</span>
                                        <button 
                                            type="button" 
                                            class="btn btn-sm btn-outline-danger ms-2"
                                            on:click={() => removeAttachment(index)}
                                        >
                                            Remove
                                        </button>
                                    </div>
                                {/each}
                            </div>
                        {/if}
                    </div>
                    {#if error}
                        <div class="alert alert-danger">{error}</div>
                    {/if}
                </form>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" on:click={onClose}>Cancel</button>
                <button 
                    type="submit" 
                    class="btn btn-primary" 
                    on:click={handleSubmit}
                    disabled={submitting}
                >
                    {#if submitting}
                        <span class="spinner-border spinner-border-sm me-2"></span>
                    {/if}
                    Send Message
                </button>
            </div>
        </div>
    </div>
</div>
<div class="modal-backdrop show"></div>
{/if}

<style>
    .modal-backdrop {
        background-color: rgba(0,0,0,0.5);
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        z-index: 1040;
    }

    .modal {
        z-index: 1050;
    }

    .modal-dialog {
        max-width: 90vw !important;
        margin: 1.75rem auto;
    }

    .modal-content {
        max-height: 90vh;
        overflow-y: auto;
    }

    .recipients-container {
        position: relative;
    }

    .recipients-dropdown {
        position: absolute;
        top: 100%;
        left: 0;
        right: 0;
        max-height: 200px;
        overflow-y: auto;
        background: white;
        border: 1px solid #dee2e6;
        border-radius: 0.375rem;
        box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        z-index: 1060;
    }

    .recipient-option {
        padding: 0.5rem 1rem;
        cursor: pointer;
        transition: background-color 0.2s;
    }

    .recipient-option:hover {
        background-color: #f8f9fa;
    }

    .selected-recipients {
        display: flex;
        flex-wrap: wrap;
        gap: 0.5rem;
    }

    .badge button {
        padding: 0.25rem;
        font-size: 0.75rem;
    }

    .attachment-list {
        display: flex;
        flex-direction: column;
        gap: 0.5rem;
    }

    .attachment-item {
        display: flex;
        align-items: center;
        padding: 0.5rem;
        background-color: #f8f9fa;
        border-radius: 0.375rem;
    }
</style>