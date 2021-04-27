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
            async hoverParams => {
                try {
                    const uri = vscode.Uri.parse(hoverParams.textDocument.uri);
                    const position = new vscode.Position(hoverParams.position.line, hoverParams.position.character);
                    const hoverModel = await vscode.commands.executeCommand('vscode.executeHoverProvider', uri, position);
                    return hoverModel;
                } catch (error) {
                    console.log(error);
                }
            });


        this.serverClient.onRequest(
            'textDocument/completion',
            async completionParams => {
                try {
                    const uri = vscode.Uri.parse(completionParams.textDocument.uri);
                    const position = new vscode.Position(completionParams.position.line, completionParams.position.character);
                    const hoverModel = await vscode.commands.executeCommand('vscode.executeCompletionItemProvider', uri, position, completionParams.context.triggerCharacter);
                    return hoverModel;
                } catch (error) {
                    console.log(error);
                }
            });
    }

    dispose() {
        this.fileSystemSpecExpansion.dispose();
    }
}