<script lang="ts">
    import { onMount } from 'svelte';
    import { getLastMessagesByDate, type MessageResponse } from '$lib/api/Messages/MessagesRequests';
    import MessagesList from '$lib/components/messages/MessagesList.svelte';
    import NewMessage from '$lib/components/messages/NewMessage.svelte';
    import MessagesContainer from '$lib/components/messages/MessagesContainer.svelte';
    import StudentDashboard from '$lib/components/student/StudentDashboard.svelte';
    import StudentAssignmentView from '$lib/components/student/StudentAssignmentView.svelte';

    let messages = [];
    let error = '';
    let loading = true;
    let showNewMessage = false;
    let showSuccessAlert = false;
    let showMessages = true;
    let activeComponent = 'dashboard';
    let selectedAssignmentId = '';

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

    const handleViewSession = (event) => {
    };
    
    const handleViewAssignment = (event) => {
        selectedAssignmentId = event.detail.assignmentId;
        setActiveComponent('assignment');
    };
    
    const handleViewLecture = (event) => {
    };

    const handleBackFromAssignment = () => {
        setActiveComponent('dashboard');
    };
</script>

{#if showSuccessAlert}
    <div class="alert alert-success alert-dismissible fade show" role="alert">
        Message sent successfully!
        <button type="button" class="btn-close" on:click={() => showSuccessAlert = false}></button>
    </div>
{/if}

<div class="page-container">
    <nav class="student-nav bg-light border-bottom py-2">
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

    <div class="content-wrapper">
        <div class="main-area {showMessages ? 'with-messages' : ''}">
            {#if activeComponent === 'dashboard'}
                <div class="p-3">
                    <StudentDashboard 
                        on:viewSession={handleViewSession}
                        on:viewAssignment={handleViewAssignment}
                        on:viewLecture={handleViewLecture}
                    />
                </div>
            {/if}
            {#if activeComponent === 'assignment'}
                <StudentAssignmentView 
                    assignmentId={selectedAssignmentId}
                    onBack={handleBackFromAssignment}
                />
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
    .page-container {
        height: 100%;
        display: flex;
        flex-direction: column;
    }

    .student-nav {
        position: sticky;
        top: 0;
        z-index: 1020;
    }

    .content-wrapper {
        flex: 1;
        display: flex;
        overflow: hidden;
    }

    .main-area {
        flex: 1;
        overflow-y: auto;
        transition: width 0.3s ease;
    }

    .main-area.with-messages {
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