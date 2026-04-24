// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import fsharpOracle from 'starlight-fsharp-oracle';
import starlightChangelogs, { makeChangelogsSidebarLinks } from 'starlight-changelogs';
import fsharpLiterate from 'starlight-fsharp-literate';
import starlightLinksValidator from 'starlight-links-validator'

import { fileURLToPath } from 'node:url';
import { resolve, dirname } from 'node:path';

const __dirname = dirname(fileURLToPath(import.meta.url));

// https://astro.build/config
export default defineConfig({
    integrations: [
        fsharpLiterate(),
        starlight({
            title: 'Fable Scriptorium',
            social: [{ icon: 'github', label: 'GitHub', href: 'https://github.com/fable-hub/Fable.Scriptorium' }],
            customCss: [
                './src/styles/custom.css',
            ],
            plugins: [
                starlightChangelogs(),
                starlightLinksValidator(),
                fsharpOracle({
                    assemblies: [
                        resolve(__dirname, '../src/Fable.Ink/bin/Debug/netstandard2.1/publish/Fable.Ink.dll'),
                        resolve(__dirname, '../src/Fable.Nib/bin/Debug/netstandard2.1/publish/Fable.Nib.dll'),
                                                resolve(__dirname, '../src/Fable.Nib.Browser/bin/Debug/netstandard2.1/publish/Fable.Nib.Browser.dll'),
                        resolve(__dirname, '../src/Fable.Nib.Snapshot/bin/Debug/net10.0/publish/Fable.Nib.Snapshot.dll'),
                        resolve(__dirname, '../src/Fable.Parchment/bin/Debug/netstandard2.1/publish/Fable.Parchment.dll'),
                        resolve(__dirname, '../src/Fable.Quill/bin/Debug/netstandard2.1/publish/Fable.Quill.dll'),
                    ],
                    output: 'api',
                    sidebar: { label: 'API Reference' }
                }),
            ],
            sidebar: [
                {
                    label: 'Introduction',
                    items: [
                        'guides/why',
                        'guides/story',
                        'guides/getting-started'
                    ],
                },
                {
                    label: 'Fable.Ink',
                    items: [
                        'ink/overview'
                    ],
                },
                {
                    label: 'Fable.Parchment',
                    items: [
                        'parchment/overview',
                    ],
                },
                {
                    label: 'Fable.Nib',
                    items: [
                        'nib/overview',
                        'nib/combinators',
                        'nib/assertions',
                        'nib/extending'
                    ],
                },
                {
                    label: 'Fable.Quill',
                    items: [
                        'quill/overview',
                        'quill/test-dsl',
                        'quill/configuration',
                        'quill/runner',
                    ],
                },
                {
                    label: 'Fable.Nib.Browser',
                    items: [
                        'nib-browser/overview',
                        'nib-browser/assertions',
                        'nib-browser/browser-tests',
                    ],
                },
                {
                    label: 'Fable.Nib.Snapshot',
                    items: [
                        'nib-snapshot/overview',
                        'nib-snapshot/configuration',
                    ],
                },
                {
                    label: 'Changelogs',
                    collapsed: true, // Make the section less intrusive
                    items: [
                        ...makeChangelogsSidebarLinks([
                            { type: 'all', base: 'changelog/fable-ink', label: 'Fable.Ink' },
                            { type: 'all', base: 'changelog/fable-parchment', label: 'Fable.Parchment' },
                            { type: 'all', base: 'changelog/fable-nib', label: 'Fable.Nib' },
                            { type: 'all', base: 'changelog/fable-nib-snapshot', label: 'Fable.Nib.Snapshot' },
                            { type: 'all', base: 'changelog/fable-nib-browser', label: 'Fable.Nib.Browser' },
                            { type: 'all', base: 'changelog/fable-quill', label: 'Fable.Quill' },
                        ])
                    ]
                }
            ],
        }),
    ],
});
