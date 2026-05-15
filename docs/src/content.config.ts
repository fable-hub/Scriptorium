import { defineCollection } from 'astro:content';
import { docsLoader } from '@astrojs/starlight/loaders';
import { docsSchema } from '@astrojs/starlight/schema';
import { changelogsLoader } from 'starlight-changelogs/loader';

export const collections = {
    docs: defineCollection({ loader: docsLoader(), schema: docsSchema() }),
    changelogs: defineCollection({
        loader: changelogsLoader([
            {
                provider: 'keep-a-changelog',
                base: 'changelog/scriptorium-ink',
                changelog: '../src/Scriptorium.Ink/CHANGELOG.md'
            },
            {
                provider: 'keep-a-changelog',
                base: 'changelog/scriptorium-parchment',
                changelog: '../src/Scriptorium.Parchment/CHANGELOG.md'
            },
            {
                provider: 'keep-a-changelog',
                base: 'changelog/scriptorium-nib',
                changelog: '../src/Scriptorium.Nib/CHANGELOG.md'
            },
            {
                provider: 'keep-a-changelog',
                base: 'changelog/scriptorium-nib-snapshot',
                changelog: '../src/Scriptorium.Nib.Snapshot/CHANGELOG.md'
            },
            {
                provider: 'keep-a-changelog',
                base: 'changelog/scriptorium-nib-browser',
                changelog: '../src/Scriptorium.Nib.Browser/CHANGELOG.md'
            },
            {
                provider: 'keep-a-changelog',
                base: 'changelog/scriptorium-quill',
                changelog: '../src/Scriptorium.Quill/CHANGELOG.md'
            },
        ]),
    })
};
