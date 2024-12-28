<script lang="ts">
    import type { MessageResponse, FileResponse } from '$lib/api/Messages/MessagesRequests';
    import { getUserData } from '$lib/api/User/UserRequests';
    import type { UserResponse } from '$lib/api/User/UserRequests';
    import { onMount } from 'svelte';

    export let message: MessageResponse;
    export let show = false;
    export let onClose: () => void;

    let sender: UserResponse | null = null;

    const getDaysAgo = (date: Date) => {
        const now = new Date();
        const diffTime = Math.abs(now.getTime() - new Date(date).getTime());
        return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    };

    const downloadFile = (file: FileResponse) => {
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

    const isImage = (file: FileResponse) => {
        return file.contentType.startsWith('image/');
    };

    onMount(async () => {
        const token = localStorage.getItem('token');
        if (token) {
            sender = await getUserData(token);
        }
    });
</script>

{#if show}
<div class="modal show d-block" tabindex="-1">
    <div class="modal-dialog modal-xl">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">{message.title}</h5>
                <button type="button" class="btn-close" on:click={onClose}></button>
            </div>
            <div class="modal-body">
                <div class="sender-info mb-3">
                    <p>From: {sender?.firstName} {sender?.lastName}</p>
                    <p><small class="text-muted">{getDaysAgo(message.createdOn)} days ago</small></p>
                </div>
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
                <button type="button" class="btn btn-secondary" on:click={onClose}>Close</button>
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

    .attachment-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
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
    }

    .filename-container {
        width: 100%;
        text-align: center;
        margin: 0.5rem 0;
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
        max-height: 200px;
        object-fit: contain;
        margin-bottom: 0.5rem;
    }
</style>