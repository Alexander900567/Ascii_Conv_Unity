using System;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{

    public GameObject grid_text_col;
    public UIManager ui_manager;
    public RectTransform canvas_transform;
    public GameObject example_grid_col;
    public int col_count;
    public int row_count;
    private float col_size;
    private float row_size;
    private List<GameObject> grid_text_cols = new List<GameObject>();
    private List<List<char>> grid_array = new List<List<char>>();
    private List<(int, int, char)> preview_buffer = new List<(int, int, char)>(); //row, col, input

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col_size = (Screen.width - ui_manager.ui_panel_transform.rect.width) / col_count;
        row_size = Screen.height / row_count;

        for (int row = 0; row < row_count; row++){
            grid_array.Add(new List<char>());
            for (int col = 0; col < col_count; col++){
                grid_array[row].Add(' ');
            }
        }

        for (int col = 0; col < col_count; col++){
            grid_text_cols.Add(Instantiate(
                grid_text_col, 
                new Vector3(
                    ui_manager.ui_panel_transform.rect.width + (col * col_size), 
                    Screen.height, 0
                ), 
                transform.rotation
            ));
            grid_text_cols[col].transform.SetParent(canvas_transform);
            grid_text_cols[col].GetComponent<RectTransform>().sizeDelta = new Vector2(col_size, Screen.height);
            /*
            Component[] components = grid_text_cols[col].GetComponents(typeof(Component));
            foreach(Component component in components) {
                Debug.Log(component.ToString());
            }
            */
        } 

        String auto_size_string = "";
        for(int row = 0; row < row_count; row++){
            auto_size_string += "N\n";
        }
        auto_size_string = auto_size_string.TrimEnd('\n');
        example_grid_col.GetComponent<TextMeshProUGUI>().text = auto_size_string;
    }

    // Update is called once per frame
    void Update()
    {
        RenderGrid();
    }
    
    void RenderGrid(){
        String col_string = "";
        float full_font_size = example_grid_col.GetComponent<TextMeshProUGUI>().fontSize;

        List<List<char>> render_array = new List<List<char>>(grid_array);
        foreach ((int, int, char) item in preview_buffer){
            render_array[item.Item1][item.Item2] = item.Item3;
        }

        for (int col = 0; col < col_count; col++){
            grid_text_cols[col].GetComponent<TextMeshProUGUI>().fontSize = full_font_size;
            col_string = "";
            for (int row = 0; row < row_count; row++){
                col_string += render_array[row][col] + "\n";
            }
            col_string = col_string.TrimEnd('\n');
            grid_text_cols[col].GetComponent<TextMeshProUGUI>().text = col_string;
        }    
    }

    public void add_to_preview_buffer(int row, int col, String input){
        Debug.Assert(input.Length == 1, "input can only be one character in add_to_preview_buffer");

        if (row >= row_count || row < 0){ return; }
        else if (col >= col_count || col < 0){ return; }

        preview_buffer.Add((row, col, input[0]));
    }

    public void write_pbuffer_to_array(){
        foreach ((int, int, char) item in preview_buffer){
            grid_array[item.Item1][item.Item2] = item.Item3;
        }
        preview_buffer.Clear();
    }

    public (int row, int col) get_grid_pos(Vector3 mouse_pos){
        int col = (int) ((mouse_pos[0] - ui_manager.ui_panel_transform.rect.width) / col_size);
        int row = (int) (mouse_pos[1] / row_size);

        if (col < 0) { col = 0; }
        else if (col >= col_count) { col = col_count - 1; }
        if (row < 0) { row = 0; }
        else if (row >= row_count) { row = row_count - 1; }

        //col = col_count - 1 - col;
        row = row_count - 1 - row;

        return (row: row, col: col);
    }

}
