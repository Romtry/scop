namespace Scop
{
    partial class Program
    {
        // ======= SHADERS 2D =======
        private static readonly string VertexShaderSource2D = @"
        #version 330 core

        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec2 aTextureCoord;

        out vec2 frag_texCoords;

        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
            frag_texCoords = aTextureCoord;
        }";

        private static readonly string FragmentShaderSource2D = @"
        #version 330 core

        uniform sampler2D uTexture;
        in vec2 frag_texCoords;

        out vec4 out_color;

        void main()
        {
            out_color = texture(uTexture, frag_texCoords);
        }";

       // ======= GÉOMÉTRIE 2D =======
        private static readonly float[] Vertices2D =
        {
             0.5f,  0.5f, 0.0f,  1.0f, 1.0f,
             0.5f, -0.5f, 0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f, 0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, 0.0f,  0.0f, 1.0f
        };

        private static readonly uint[] Indices2D =
        {
            0, 1, 3,
            1, 2, 3
        };
	}
}
