-- There is a bug in F#/Neovim which freezes the editor when using semantic tokens
-- and opening Quill.fs file
--
-- So for now, we disable semantic tokens for F# language server (fsautocomplete)
vim.api.nvim_create_autocmd("LspAttach", {
  callback = function(args)
    local client = vim.lsp.get_client_by_id(args.data.client_id)
    if client.name == "fsautocomplete" then
      client.server_capabilities.semanticTokensProvider = nil
    end
  end,
})
