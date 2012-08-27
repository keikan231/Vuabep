/*
Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    config.language = 'vi';
    //config.uiColor = '#009DC9';

    config.toolbar = 'MyToolbar';

    config.extraPlugins = 'youtube';

    config.resize_enabled = false;

    config.toolbar_MyToolbar =
	[
		{ name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', 'NumberedList', 'BulletedList', 'Table', 'Link', 'Unlink', 'Preview', 'TextColor', 'Source', 'PasteFromWord', 'RemoveFormat'] },
        { name: 'entertainments', items: ['Image','YouTube'] },
        { name: 'styles', items: ['Styles', 'Format'] }
	];
};
