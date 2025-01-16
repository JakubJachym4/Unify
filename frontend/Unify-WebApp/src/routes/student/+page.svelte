<script lang="ts">
    import { onMount } from 'svelte';
    import { getLastMessagesByDate, type MessageResponse } from '$lib/api/Messages/MessagesRequests';
    import MessagesList from '$lib/components/messages/MessagesList.svelte';
    import NewMessage from '$lib/components/messages/NewMessage.svelte';
    import MessagesContainer from '$lib/components/messages/MessagesContainer.svelte';

    let messages = [];
    let error = '';
    let loading = true;
    let showNewMessage = false;
    let showSuccessAlert = false;
    let showMessages = true;
    let activeComponent = 'dashboard';

    const handleMessageSent = () => {
        showSuccessAlert = true;
        setTimeout(() => {
            showSuccessAlert = false;
        }, 3000);
    };

    const toggleMessages = () => {
        showMessages = !showMessages;
    };

    const setActiveComponent = (component: string) => {
        activeComponent = component;
    };
</script>

{#if showSuccessAlert}
    <div class="alert alert-success alert-dismissible fade show" role="alert">
        Message sent successfully!
        <button type="button" class="btn-close" on:click={() => showSuccessAlert = false}></button>
    </div>
{/if}

<div class="student-container">
    <nav class="student-nav bg-light border-bottom mb-4">
        <div class="container-fluid">
            <div class="d-flex justify-content-between align-items-center py-3">
                <div class="nav-buttons">
                    <button 
                        class="btn {activeComponent === 'dashboard' ? 'btn-primary' : 'btn-outline-primary'} me-2"
                        on:click={() => setActiveComponent('dashboard')}
                    >
                        Dashboard
                    </button>
                    <!-- Add more student-specific buttons here -->
                </div>
                <button 
                    class="btn {showMessages ? 'btn-primary' : 'btn-outline-primary'}"
                    on:click={toggleMessages}
                >
                    {showMessages ? 'Hide Messages' : 'Show Messages'}
                </button>
            </div>
        </div>
    </nav>

    <div class="content-container">
        <div class="main-content {showMessages ? 'with-messages' : ''}">
            {#if activeComponent === 'dashboard'}
                <div class="p-3">
                    <h2>Student Dashboard</h2>
                    <!-- Add student dashboard content here -->
                </div>
            {/if}
        </div>
        
        {#if showMessages}
            <div class="messages-sidebar">
                <MessagesContainer fixed={false} />
            </div>
        {/if}
    </div>
</div>

<NewMessage 
    show={showNewMessage}
    onClose={() => showNewMessage = false}
    on:messageSent={handleMessageSent}
/>

<style>
    .student-container {
        height: calc(100vh - 56px);
        overflow: hidden;
    }

    .student-nav {
        position: sticky;
        top: 0;
        z-index: 1020;
    }

    .content-container {
        display: flex;
        height: 100%;
    }

    .main-content {
        flex: 1;
        padding: 1rem;
        overflow-y: auto;
        transition: width 0.3s ease;
    }

    .main-content.with-messages {
        width: calc(100% - 400px);
    }

    .messages-sidebar {
        width: 400px;
        border-left: 1px solid #dee2e6;
        height: 100%;
        overflow-y: auto;
    }

    @media (max-width: 768px) {
        .main-content.with-messages {
            width: 100%;
        }

        .messages-sidebar {
            position: fixed;
            right: 0;
            top: 56px;
            bottom: 0;
            background: white;
            z-index: 1030;
        }
    }
</style>