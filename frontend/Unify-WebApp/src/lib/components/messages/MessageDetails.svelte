<script lang="ts">
	import MessageDetails from './MessageDetails.svelte';
	import { globalUsers } from './../../stores/globalUsers';
    import type { MessageResponse, Attachment } from '$lib/api/Messages/MessagesRequests';
    import { getUserData } from '$lib/api/User/UserRequests';
    import type { UserResponse } from '$lib/api/User/UserRequests';
    import { onMount, onDestroy } from 'svelte';
	import { get } from 'svelte/store';
    import NewMessage from './NewMessage.svelte';
	import { messages } from '$lib/stores/messages';

    export let message: MessageResponse;
    export let show = false;
    export let onClose: () => void;
    export let hideReplyButton = false;
    export let modalLevel = 1;

    let sender: UserResponse | null = null;
    let recipientNames: string[] = [];
    let showReplyForm = false;
    let showRespondingMessage = false;
    let respondingMessage: MessageResponse | null = null;

    const getDaysAgo = (date: Date) => {
        const now = new Date();
        const diffTime = Math.abs(now.getTime() - new Date(date).getTime());
        return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    };

    const downloadFile = (file: Attachment) => {
        const byteCharacters = atob(file.data);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        const blob = new Blob([byteArray], { type: file.contentType });
        
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = file.fileName;
        link.click();
        window.URL.revokeObjectURL(link.href);
    };

    const isImage = (file: Attachment) => {
        return file.contentType.startsWith('image/');
    };

    const handleKeydown = (event: KeyboardEvent) => {
        if (event.key === 'Escape' && show) {
            onClose();
        }
    };

    onMount(async () => {
        window.addEventListener('keydown', handleKeydown);

        const users = get(globalUsers);
        sender = users.find(u => u.id === message.senderId) || null;
        
        // Get recipient names from globalUsers store
        recipientNames = message.recipientsIds
            .map(id => users.find(u => u.id === id))
            .filter((u): u is UserResponse => u !== undefined)
            .map(u => `${u.firstName} ${u.lastName}`);

        if (message.respondingToId) {
            const allMessages = get(messages).messages;
            respondingMessage = allMessages.find(m => m.messageId === message.respondingToId) || null;
        }
    });

    onDestroy(() => {
        window.removeEventListener('keydown', handleKeydown);
    });

    $: displayedRecipients = recipientNames.slice(0, 10).join(', ');
    $: remainingCount = Math.max(0, recipientNames.length - 10);
    $: allRecipients = recipientNames.join(', ');
</script>

{#if show}
<div class="modal show d-block" tabindex="-1" style="z-index: {1050 + modalLevel}">
    <div class="modal-dialog modal-xl">
        <div class="modal-content">
            <div class="modal-header">
                <div>
                    <h5 class="modal-title mb-0">{message.title}</h5>
                    {#if message.respondingToId && respondingMessage}
                        <small class="text-muted">
                            In response to: <span 
                                class="responding-to-link"
                                role="button"
                                on:click={() => showRespondingMessage = true}
                              >
                                {respondingMessage.title}
                              </span>
                        </small>
                    {/if}
                    <small class="text-muted">{getDaysAgo(message.createdOn)} days ago</small>
                </div>
                <button type="button" class="btn-close" on:click={onClose}></button>
            </div>
            <div class="modal-body">
                <div class="message-header mb-1">
                    <p class="mb-2">From: {sender?.firstName} {sender?.lastName}</p>
                    <div class="recipients-container">
                        <p class="mb-0">
                            To: <span class="recipients" title={allRecipients}>
                                {displayedRecipients}
                                {#if remainingCount > 0}
                                    <span class="text-muted">
                                        and {remainingCount} more
                                    </span>
                                {/if}
                            </span>
                        </p>
                    </div>
                </div>
                <hr class="message-divider"/>
                <div class="message-content mb-4">
                    <p>{message.content}</p>
                </div>
                {#if message.attachments && message.attachments.length > 0}
                    <div class="attachments">
                        <h6>Attachments:</h6>
                        <div class="attachment-grid">
                            {#each message.attachments as file}
                                <div class="attachment-item">
                                    {#if isImage(file)}
                                        <img 
                                            src={`data:${file.contentType};base64,${file.data}`} 
                                            alt={file.fileName}
                                            class="attachment-preview"
                                        />
                                    {/if}
                                    <div class="filename-container">
                                        <span class="filename" title={file.fileName}>
                                            {file.fileName}
                                        </span>
                                    </div>
                                    <button 
                                        class="btn btn-sm btn-outline-primary mt-2"
                                        on:click={() => downloadFile(file)}
                                    >
                                        Download
                                    </button>
                                </div>
                            {/each}
                        </div>
                    </div>
                {/if}
            </div>
            <div class="modal-footer">
                {#if !hideReplyButton}
                    <button 
                        type="button" 
                        class="btn btn-primary"
                        on:click={() => showReplyForm = true}
                    >
                        Reply
                    </button>
                {/if}
                <button type="button" class="btn btn-secondary" on:click={onClose}>Close</button>
            </div>
        </div>
    </div>
</div>
<div class="modal-backdrop show" style="z-index: {1040 + modalLevel}"></div>
{/if}

{#if showReplyForm}
    <NewMessage 
        show={true}
        onClose={() => showReplyForm = false}
        respondingToId={message.messageId}
        modalLevel={modalLevel + 1}
    />
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

    .attachment-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
        gap: 1rem;
        margin-top: 1rem;
    }

    .attachment-item {
        display: flex;
        flex-direction: column;
        align-items: center;
        padding: 1rem;
        border: 1px solid #dee2e6;
        border-radius: 0.375rem;
        width: 100%;
        min-height: 200px;
        justify-content: space-between;
    }

    .filename-container {
        width: 100%;
        text-align: center;
        margin: 0.5rem 0;
        flex-grow: 1;
    }

    .filename {
        display: block;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        max-width: 200px;
    }

    .attachment-preview {
        max-width: 100%;
        max-height: 150px;
        object-fit: contain;
        margin-bottom: 0.5rem;
    }

    .recipients-container {
        position: relative;
    }

    .recipients {
        cursor: help;
    }

    .recipients:hover {
        text-decoration: underline dotted;
    }

    .message-header {
    }

    .message-divider {
        margin: 1rem 0;
        border-top: 1px solid var(--bs-border-color);
    }

    .modal-title {
        font-size: 1.25rem;
    }

    .responding-to-link {
        color: var(--bs-primary);
        text-decoration: underline;
        cursor: pointer;
    }

    .responding-to-link:hover {
        color: var(--bs-primary-dark);
    }
</style>