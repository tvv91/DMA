// Rich Text Editor functionality
$(document).ready(function() {
    const $editor = $('#content-editor');
    const $hiddenTextarea = $('#content');
    
    // Only initialize if editor exists (on create/edit pages)
    if ($editor.length === 0) {
        return;
    }
    
    // Initialize editor with existing content from textarea
    if ($hiddenTextarea.val() && $hiddenTextarea.val().trim()) {
        $editor.html($hiddenTextarea.val());
    }
    
    // Sync editor content to hidden textarea
    function syncContent() {
        const content = $editor.html();
        $hiddenTextarea.val(content);
        // Trigger change event for form validation
        $hiddenTextarea.trigger('input');
    }
    
    // Update on input
    $editor.on('input', syncContent);
    $editor.on('paste', function(e) {
        // Allow paste, then sync
        setTimeout(function() {
            syncContent();
            // Clean up pasted content
            const pastedContent = $editor.html();
            $editor.html(pastedContent);
        }, 10);
    });
    
    // Toolbar button handlers
    $('.toolbar-btn').on('click', function(e) {
        e.preventDefault();
        const command = $(this).data('command');
        const value = $(this).data('value');
        
        $editor.focus();
        
        if (command === 'createLink') {
            const url = prompt('Enter URL:');
            if (url) {
                document.execCommand('createLink', false, url);
            }
        } else if (value) {
            document.execCommand(command, false, value);
        } else {
            document.execCommand(command, false, null);
        }
        
        syncContent();
        updateToolbarState();
    });
    
    // Handle keyboard shortcuts
    $editor.on('keydown', function(e) {
        // Ctrl+B for bold
        if (e.ctrlKey && e.key === 'b') {
            e.preventDefault();
            document.execCommand('bold', false, null);
            syncContent();
            updateToolbarState();
        }
        // Ctrl+I for italic
        if (e.ctrlKey && e.key === 'i') {
            e.preventDefault();
            document.execCommand('italic', false, null);
            syncContent();
            updateToolbarState();
        }
        // Ctrl+U for underline
        if (e.ctrlKey && e.key === 'u') {
            e.preventDefault();
            document.execCommand('underline', false, null);
            syncContent();
            updateToolbarState();
        }
    });
    
    // Update button states based on selection
    function updateToolbarState() {
        $('.toolbar-btn').each(function() {
            const command = $(this).data('command');
            if (command && document.queryCommandSupported && document.queryCommandSupported(command)) {
                try {
                    const isActive = document.queryCommandState(command);
                    $(this).toggleClass('active', isActive);
                } catch (e) {
                    // Ignore errors for unsupported commands
                }
            }
        });
    }
    
    $editor.on('mouseup keyup', function() {
        updateToolbarState();
    });
    
    // Initial sync
    syncContent();
    
    // Sync before form submit
    $('form').on('submit', function(e) {
        syncContent();
        // Ensure content is set
        if (!$hiddenTextarea.val() || !$hiddenTextarea.val().trim()) {
            $hiddenTextarea.val($editor.html());
        }
    });
    
    // Sync on blur
    $editor.on('blur', syncContent);
});

