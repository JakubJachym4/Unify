<script lang="ts">
    import { onMount } from 'svelte';
    import type { MessageResponse } from '$lib/api/Messages/MessagesRequests';
    import { getLastMessagesByNumber } from '$lib/api/Messages/MessagesRequests';
    import MessagesList from './MessagesList.svelte';
    import NewMessage from './NewMessage.svelte';
    import { messagesStore } from '$lib/stores/messages';
    import { get } from 'svelte/store';

    export let fixed = false;
    let messagesList: MessageResponse[] = [];
    let allMessages: MessageResponse[] = [];
    let error = '';
    let loading = true;
    let showNewMessage = false;
    let showAllMessages = false;
    let loadingAllMessages = false;

    const loadMessages = async () => {
        try {
            const lastWeek = new Date();
            lastWeek.setDate(lastWeek.getDate() - 7);
            const date = `${lastWeek.getFullYear()}-${lastWeek.getMonth() + 1}-${lastWeek.getDate()}`;
            await messagesStore.refresh(date);
            messagesList = get(messagesStore).messages;
            loading = false;
        } catch (err) {
            error = (err as Error).message;
            loading = false;
        }
    };

    const loadAllMessages = async () => {
        try {
            loadingAllMessages = true;
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            allMessages = await getLastMessagesByNumber(1000, token)|| [];
            console.log(allMessages);
            loadingAllMessages = false;

        } catch (err) {
            error = (err as Error).message;
            loadingAllMessages = false;
        }
    };

    $: {
        $messagesStore;  // Subscribe to store changes
        if (!loading) {
            messagesList = $messagesStore;
        }
    }

    onMount(() => {
        loadMessages();
        return () => {};
    });
</script>

<div class="messages-panel" class:fixed>
    <div class="messages-container">
        <div class="d-flex justify-content-between align-items-center mb-3">
            <h2>Messages</h2>
        </div>
        <button 
                class="btn btn-outline-primary btn-sm mb-2"
                on:click={() => {
                    showAllMessages = true;
                    loadAllMessages();
                }}
            >
                Show All Messages
            </button>
        <button 
            class="btn btn-primary mb-3 w-100" 
            on:click={() => showNewMessage = true}
        >
            New Message
        </button>
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
            <div class="messages-list">
                <MessagesList messages={messagesList} />
            </div>
        {/if}
    </div>
</div>

<!-- All Messages Modal -->
{#if showAllMessages}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog modal-xl modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">All Messages</h5>
                    <button 
                        type="button" 
                        class="btn-close" 
                        on:click={() => showAllMessages = false}
                    ></button>
                </div>
                <div class="modal-body">
                    {#if loadingAllMessages}
                        <div class="d-flex justify-content-center">
                            <div class="spinner-border" role="status">
                                <span class="visually-hidden">Loading...</span>
                            </div>
                        </div>
                    {:else}
                        <div class="all-messages-list">
                            <MessagesList messages={{ messages: allMessages.messages }} />
                        </div>
                    {/if}
                </div>
                <div class="modal-footer">
                    <button 
                        type="button" 
                        class="btn btn-secondary" 
                        on:click={() => showAllMessages = false}
                    >
                        Close
                    </button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<NewMessage 
    show={showNewMessage}
    onClose={() => showNewMessage = false}
/>

<style>
    .messages-panel {
        background: white;
        border-left: 1px solid #dee2e6;
        height: 100%;
    }

    .messages-panel.fixed {
        position: fixed;
        right: 0;
        top: 56px;
        height: calc(100vh - 56px);
        padding-top: 1rem;
    }

    .messages-container {
        padding: 1rem;
        height: 100%;
        display: flex;
        flex-direction: column;
    }

    .messages-list {
        overflow-y: auto;
        flex: 1;
        padding-right: 0.5rem;
    }

    .all-messages-list {
        max-height: 70vh;
        overflow-y: auto;
    }

    /* Modal styles */
    .modal-xl {
        max-width: 90vw;
    }

    .modal-body {
        padding: 1rem;
        background-color: #f8f9fa;
    }
</style>