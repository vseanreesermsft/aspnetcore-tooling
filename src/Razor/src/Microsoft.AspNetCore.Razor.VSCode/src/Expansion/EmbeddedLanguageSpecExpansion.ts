/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

import * as vscode from 'vscode';
import { RazorLanguageServerClient } from '../RazorLanguageServerClient';
import { FileSystemSpecExpansion } from './FileSystemSpecExpansion';

export class EmbeddedLanguageSpecExpansion implements vscode.Disposable {
    private readonly fileSystemSpecExpansion: FileSystemSpecExpansion;

    constructor(
        private readonly serverClient: RazorLanguageServerClient) {
        this.fileSystemSpecExpansion = new FileSystemSpecExpansion(serverClient);
    }

    public async initialize() {
        await this.fileSystemSpecExpansion.initialize();
    }

    public register() {
        this.fileSystemSpecExpansion.register();
        this.serverClient.onRequest(
            'textDocument/open',
            async openTextDocumentParameters => {
                try {
                    const uri = vscode.Uri.parse(openTextDocumentParameters.textDocument.uri);
                    await vscode.workspace.openTextDocument(uri)
                } catch (error) {
                    throw error;
                }
            });

        this.serverClient.onRequest(
            'textDocument/close',
            async closeTextDocumentParameters => console.log(`Should be closing the text document ${closeTextDocumentParameters.textDocument.uri}`));

        this.serverClient.onRequest(
            'textDocument/hover',
            async hoverParams => vscode.commands.executeCommand('vscode.executeHoverProvider', hoverParams));


        this.serverClient.onRequest(
            'textDocument/completion',
            async completionParams => vscode.commands.executeCommand('vscode.executeCompletionItemProvider', completionParams));
    }

    dispose() {
        this.fileSystemSpecExpansion.dispose();
    }
}