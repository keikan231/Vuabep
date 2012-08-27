CKEDITOR.plugins.add('youtube',
{
    init: function (editor) {

        var pluginName = 'youtube';

        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));
        editor.ui.addButton('YouTube',
            {
                label: 'YouTube',
                command: pluginName,
                icon: this.path + 'images/youTube.png'
            });

            CKEDITOR.dialog.add(pluginName, this.path + 'dialogs/' + pluginName + '.js');


    }
});
