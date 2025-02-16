<script lang="ts">
	import { replyToMessage, sendMessage, type MessageResponse, type ReplyToMessageRequest, type SendMessageRequest, forwardMessage, SeverityLevel, type SendNotificationRequest, sendNotification } from './../../api/Messages/MessagesRequests';
    import { onMount, onDestroy } from 'svelte';
    import type { UserResponse } from '$lib/api/User/UserRequests';
    import { getUserData } from '$lib/api/User/UserRequests';
	import type { ApiRequestError } from '$lib/api/apiError.js';
	import { api } from '$lib/api/api.js';
    import { globalUsers } from '$lib/stores/globalUsers';
    import { get } from 'svelte/store';
	import { browser } from '$app/environment';
    import { messagesStore } from '$lib/stores/messages';
    import MessageDetails from './MessageDetails.svelte';
    import ForwardMessage from './ForwardMessage.svelte';
    import { user } from '$lib/stores/user';
	import { isActionFailure } from '@sveltejs/kit';
	import { goto } from '$app/navigation';

    export let show = false;
    export let onClose: () => void;
    export let modalLevel = 1;
    export let respondingToId: string | null = null;

    let title =  respondingToId ? 'Re: ' : '';
    let content = '';
    let recipientsIds: string[] = [];
    let attachments: File[] = [];
    let error = '';
    let submitting = false;

    let availableUsers: UserResponse[] = [];
    let searchTerm = '';
    let showDropdown = false;

    let fileInput: HTMLInputElement;
    let dropdownRef: HTMLDivElement;

    let showRespondingMessage = false;
    let respondingMessage: MessageResponse | null = null;

    let showForwardDropdown = false;
    let forwardingMessageId: string | null = null;
    let showForwardForm = false;

    let forwardRecipients: string[] = [];
    let showForwardDialog = false;

    let isNotification = false;
    let severityLevel: SeverityLevel = SeverityLevel.Information;
    let expirationDate = new Date();
    expirationDate.setDate(expirationDate.getDate() + 7); // Default 7 days


    const canSendNotification = () : boolean =>
    {
        return $user != null && ($user.roles.includes('Lecturer') ||
        $user.roles.includes('Administrator')) && !respondingToId;
    }

    $: filteredUsers = availableUsers.filter(user => 
        !recipientsIds.includes(user.id) &&
        (user.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
         user.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
         user.email.toLowerCase().includes(searchTerm.toLowerCase()))
    );

    $: {
        if (respondingToId) {
            const allMessages = get(messagesStore).messages;
            respondingMessage = allMessages.find(m => m.messageId === respondingToId) || null;
            if (!recipientsIds.includes(respondingMessage?.senderId || '')){
                recipientsIds.push(respondingMessage?.senderId || '');
            }
        
        }
    }

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

    const handleMessageSent = async (messageId: string) => {
        forwardingMessageId = messageId;
        showForwardForm = true;
    };

    const handleForward = async () => {
        if (!forwardingMessageId || forwardRecipients.length === 0) {
            error = 'Please select recipients for forwarding';
            return;
        }

        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            
            await forwardMessage({
                originalMessageId: forwardingMessageId,
                newRecipientsIds: forwardRecipients
            }, token);

            // Refresh messages and close
            const lastWeek = new Date();
            lastWeek.setDate(lastWeek.getDate() - 7);
            const date = `${lastWeek.getFullYear()}-${lastWeek.getMonth() + 1}-${lastWeek.getDate()}`;
            await messagesStore.refresh(date);
            
            showForwardDropdown = false;
            onClose();
        } catch (err) {
            error = (err as ApiRequestError).details;
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
            
            let messageId;
            if (isNotification) {
                const request: SendNotificationRequest = {
                    title,
                    content,
                    recipientsIds,
                    attachments,
                    severity: severityLevel.toString(),
                    expirationDate
                };
                console.log(request)
                messageId = await sendNotification(request, token);
            } else if (respondingToId) {
                const request: ReplyToMessageRequest = {
                    title,
                    content,
                    recipientsIds,
                    attachments,
                    respondingToId
                };
                messageId = await replyToMessage(request, token);
            } else {
                const request: SendMessageRequest = {
                    title,
                    content,
                    recipientsIds,
                    attachments
                };
                messageId = await sendMessage(request, token);
            }
            
            if (forwardRecipients.length > 0) {
                await forwardMessage({
                    originalMessageId: messageId,
                    newRecipientsIds: forwardRecipients
                }, token);
            }

            const lastWeek = new Date();
            lastWeek.setDate(lastWeek.getDate() - 7);
            const date = `${lastWeek.getFullYear()}-${lastWeek.getMonth() + 1}-${lastWeek.getDate()}`;
            await messagesStore.refresh(date);
            goto('/')
            onClose();
        } catch (err) {
            error = (err as ApiRequestError).details;
        } finally {
            submitting = false;
        }
    };

    const handleClickOutside = (event: MouseEvent) => {
        if (dropdownRef && !dropdownRef.contains(event.target as Node)) {
            showDropdown = false;
        }
    };

    const handleForwardRecipientsSelected = (event: CustomEvent<string[]>) => {
        forwardRecipients = event.detail;
        showForwardDialog = false;
    };

    onMount(() => {
        if(browser){
            window.addEventListener('keydown', handleKeydown);
            const users = get(globalUsers);
            availableUsers = users;
            document.addEventListener('click', handleClickOutside);
        }
    });

    onDestroy(() => {
        if(browser){
            window.removeEventListener('keydown', handleKeydown);
            document.removeEventListener('click', handleClickOutside);
        }
    });

    const removeRecipient = (id: string) => {
        recipientsIds = recipientsIds.filter(recipientId => recipientId !== id);
    };

    const addRecipient = (user: UserResponse) => {
        if (!recipientsIds.includes(user.id)) {
            recipientsIds = [...recipientsIds, user.id];
        }
        searchTerm = '';
        showDropdown = false;
    };
</script>

{#if show}
<div class="modal show d-block" tabindex="-1" style="z-index: {1050 + modalLevel}">
    <div class="modal-dialog modal-xl">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">{respondingToId ? 'Reply to Message' : 'New Message'}</h5>
                <!-- svelte-ignore a11y_consider_explicit_label -->
                <button type="button" class="btn-close" on:click={onClose}></button>
            </div>
            <div class="modal-body">
                {#if respondingMessage}
                    <div class="responding-to-container mb-3">
                        <h6 class="mb-2">Responding to:</h6>
                        <div 
                            class="responding-to-preview"
                            role="button"
                            on:click={() => showRespondingMessage = true}
                        >
                            <h6>{respondingMessage.title}</h6>
                            <p class="text-muted mb-0">{respondingMessage.content.substring(0, 100)}...</p>
                        </div>
                    </div>
                {/if}
                <form on:submit|preventDefault={handleSubmit}>
                    <div class="mb-3">
                        <label for="title" class="form-label">Title</label>
                        <input type="text" class="form-control" id="title" bind:value={title} required>
                    </div>
                    {#if canSendNotification()}
                        <div class="mb-3 form-check">
                            <input 
                                type="checkbox" 
                                class="form-check-input" 
                                id="isNotification"
                                bind:checked={isNotification}
                            >
                            <label class="form-check-label" for="isNotification">
                                Send as Notification
                            </label>
                        </div>

                        {#if isNotification}
                            <div class="row mb-3">
                                <div class="col">
                                    <label for="severityLevel" class="form-label">Severity Level</label>
                                    <select 
                                        class="form-select" 
                                        id="severityLevel"
                                        bind:value={severityLevel}
                                    >
                                        <option value={SeverityLevel.Critical}>Critical</option>
                                        <option value={SeverityLevel.Important}>Important</option>
                                        <option value={SeverityLevel.Information}>Information</option>
                                        <option value={SeverityLevel.Notification}>Notification</option>
                                    </select>
                                </div>
                                <div class="col">
                                    <label for="expirationDate" class="form-label">Expires On</label>
                                    <input 
                                        type="date" 
                                        class="form-control" 
                                        id="expirationDate"
                                        bind:value={expirationDate}
                                        min={new Date().toISOString().split('T')[0]}
                                    >
                                </div>
                            </div>
                        {/if}
                    {/if}
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
                                            on:click={() => addRecipient(user)}
                                        >
                                            {user.firstName} {user.lastName}
                                        </button>
                                    {/each}
                                </div>
                            {/if}
                        </div>
                    </div>
                    {#if forwardRecipients.length > 0}
                        <div class="mb-3">
                            <label class="form-label">Forward Recipients</label>
                            <div class="selected-recipients mb-2">
                                {#each forwardRecipients as recipientId}
                                    {#each availableUsers.filter(u => u.id === recipientId) as user}
                                        <span class="badge bg-success me-2 mb-2">
                                            {user.firstName} {user.lastName}
                                            <button 
                                                type="button" 
                                                class="btn-close btn-close-white ms-2"
                                                on:click={() => forwardRecipients = forwardRecipients.filter(id => id !== user.id)}
                                            ></button>
                                        </span>
                                    {/each}
                                {/each}
                            </div>
                        </div>
                    {/if}
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
                {#if !isNotification}
                    <button type="button" class="btn btn-secondary" on:click={() => showForwardDialog = true}>
                        Add Forward Recipients
                    </button>
                {/if}
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
                    {isNotification ? 'Send Notification' : 'Send Message'}
                </button>
            </div>
        </div>
    </div>
</div>
<div class="modal-backdrop show"></div>
{/if}


{#if showRespondingMessage && respondingMessage}
    <MessageDetails 
        message={respondingMessage}
        show={true}
        onClose={() => showRespondingMessage = false}
        modalLevel={modalLevel + 1}
        hideReplyButton={true}
    />
{/if}

{#if showForwardDialog}
    <ForwardMessage 
        show={true}
        onClose={() => showForwardDialog = false}
        modalLevel={modalLevel + 1}
        shouldDispatch={true}
        on:recipientsSelected={handleForwardRecipientsSelected}
    />
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

    .responding-to-container {
        border-left: 4px solid var(--bs-primary);
        padding-left: 1rem;
    }

    .responding-to-preview {
        background-color: #f8f9fa;
        padding: 1rem;
        border-radius: 0.375rem;
        cursor: pointer;
        transition: background-color 0.2s;
    }

    .responding-to-preview:hover {
        background-color: #e9ecef;
    }
</style>