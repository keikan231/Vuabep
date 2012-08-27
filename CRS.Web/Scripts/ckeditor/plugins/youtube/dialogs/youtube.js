CKEDITOR.dialog.add('youtube', function (editor) {
    CKEDITOR.skins.load(editor, 'youtube');
    return {
        title: 'YouTube',
        minWidth: 300,
        minHeight: 100,
        contents: [
			{
			    id: 'plugin_text',
			    label: '',
			    title: '',
			    expand: true,
			    padding: 0,
			    elements:
				[
					{
					    type: 'html',
					    html: 'Thêm địa chỉ youtube vào ô phía dưới' + '</p>'
					},
                    {
                        type: 'text',
                        id: 'url',
                        label: 'URL',
                        validate: CKEDITOR.dialog.validate.notEmpty('Xin vui lòng đưa đúng đường dẫn Youtube video'),
                        required: true,
                        commit: function (data) {
                            data.url = this.getValue();
                        }
                    },
				]
			}
		],
        onOk: function () {
            // Create a link element and an object that will store the data entered in the dialog window.
            // http://docs.cksource.com/ckeditor_api/symbols/CKEDITOR.dom.document.html#createElement
            var dialog = this,
				data = {},
                iFrameElement = editor.document.createElement('iframe');

            // Populate the data object with data entered in the dialog window.
            // http://docs.cksource.com/ckeditor_api/symbols/CKEDITOR.dialog.html#commitContent
            this.commitContent(data);           

            var width = 480;
            var heigth = 385;

            //http://www.youtube.com/watch?v=rLN8M9ZLeg0
            //http://www.youtube.com/watch?v=qSqLTTs1tNE&feature=topvideos_people
            //change to 
            //http://www.youtube.com/embed/rLN8M9ZLeg0
            var src = data.url.replace("watch?v=", "embed/").split("&",1)
            //.replace("&feature=related", "");
            //<iframe title='YouTube video player' type='text/html' width='{0}' height='{1}' src='{2}' frameborder='0' ></iframe>"           

            iFrameElement.setAttribute('title', 'YouTube video player');
            iFrameElement.setAttribute('type', 'text/html');
            iFrameElement.setAttribute('width', width);
            iFrameElement.setAttribute('height', heigth);
            iFrameElement.setAttribute('src', src);
            iFrameElement.setAttribute('frameborder', '0');
            editor.insertElement(iFrameElement);

        }
    };
});